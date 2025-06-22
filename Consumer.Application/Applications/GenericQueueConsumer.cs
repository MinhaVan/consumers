using System.Text;
using System.Text.Json;
using Consumer.Application.Extensions;
using Consumer.Domain.Configuration;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.Utils;
using Consumer.Domain.ViewModels.Localizacao;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Application.Applications;

public class GenericQueueConsumer<T> : BackgroundService
{
    private readonly IModel _channel;
    private readonly ILogger _logger;
    private readonly IConnection _connection;
    private readonly GeneralSetting _generalSetting;
    private readonly IQueueMessageHandler<T> _handler;
    private readonly string _queueName;
    private readonly string _retryQueueName;
    private readonly string _deadQueueName;

    public GenericQueueConsumer(
        ILogger<GenericQueueConsumer<T>> logger,
        GeneralSetting generalSetting,
        IQueueMessageHandler<T> handler,
        string queueName)
    {
        _logger = logger;
        _generalSetting = generalSetting;
        _handler = handler;
        _queueName = queueName;
        _retryQueueName = $"{queueName}.retry";
        _deadQueueName = $"{queueName}.dead";

        var factory = new ConnectionFactory
        {
            HostName = _generalSetting.RabbitMq.HostName,
            Port = _generalSetting.RabbitMq.Port,
            UserName = _generalSetting.RabbitMq.UserName,
            Password = _generalSetting.RabbitMq.Password,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _retryQueueName }
        });

        _channel.QueueDeclare(queue: _retryQueueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _queueName },
            { "x-message-ttl", 5 * 60 * 1000 }
        });

        _channel.QueueDeclare(queue: _deadQueueName, durable: true, exclusive: false, autoDelete: false);
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
                var mensagem = JsonSerializer.Deserialize<BaseQueue<T>>(json);

                var result = await _handler.HandleAsync(mensagem);

                if (result is not null && result.Sucesso)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogError($"Erro lógico: {JsonSerializer.Serialize(result)}");
                    _channel.RetryMessage<T>(ea, _generalSetting, _logger, _deadQueueName, _retryQueueName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no processamento da fila");
                _channel.RetryMessage<T>(ea, _generalSetting, _logger, _deadQueueName, _retryQueueName);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}