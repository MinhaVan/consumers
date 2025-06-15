using System.Text;
using System.Text.Json;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.Utils;
using Consumer.Domain.ViewModels.Localizacao;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Localizacao.Consumers;

public class QueueConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly ILogger<QueueConsumer> _logger;
    private readonly IConnection _connection;
    private readonly ILocalizacaoApplication _localizacaoApplication;
    private readonly RabbitMqSettings _settings;
    //
    private const string QueueName = RabbitMqQueues.EnviarLocalizacao;
    private readonly string RetryQueueName = $"{QueueName}.retry";
    private readonly string DeadQueueName = $"{QueueName}.dead";

    public QueueConsumer(
        ILocalizacaoApplication localizacaoApplication,
        ILogger<QueueConsumer> logger,
        IOptions<RabbitMqSettings> options)
    {
        _logger = logger;
        _settings = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Fila principal
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", RetryQueueName }
        });

        // Fila de retry com TTL de 5 minutos
        _channel.QueueDeclare(queue: RetryQueueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", QueueName },
            { "x-message-ttl", 5 * 60 * 1000 }
        });

        // Fila de mensagens mortas
        _channel.QueueDeclare(queue: DeadQueueName, durable: true, exclusive: false, autoDelete: false);

        _localizacaoApplication = localizacaoApplication;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var mensagem = JsonSerializer.Deserialize<BaseQueue<EnviarLocalizacaoWebSocketRequest>>(json);

                var localizacaoResponse = await _localizacaoApplication.SaveLocalizacaoAsync(mensagem);

                if (localizacaoResponse is not null && localizacaoResponse.Sucesso)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogError($"Falha lógica: {JsonSerializer.Serialize(localizacaoResponse)}");
                    RetryMessage(ea);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no processamento: {ex.Message}");
                RetryMessage(ea);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private void RetryMessage(BasicDeliverEventArgs ea)
    {
        // Primeiro faz o ACK da mensagem atual
        _channel.BasicAck(ea.DeliveryTag, false);

        // Cria nova mensagem e envia para a fila de retry
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: RetryQueueName,
            basicProperties: props,
            body: ea.Body);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
