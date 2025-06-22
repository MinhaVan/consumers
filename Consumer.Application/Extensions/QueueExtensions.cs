using System.Text;
using System.Text.Json;
using Consumer.Domain.Configuration;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Application.Extensions;

public static class QueueExtensions
{
    public static void RetryMessage<T>(this IModel channel, BasicDeliverEventArgs ea, GeneralSetting generalSetting, ILogger logger, string deadQueueName, string retryQueueName)
    {
        channel.BasicAck(ea.DeliveryTag, false);

        int retryCount = 0;

        if (ea.BasicProperties.Headers != null &&
            ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var value))
        {
            var stringValue = Encoding.UTF8.GetString((byte[])value);
            if (!int.TryParse(stringValue, out retryCount))
            {
                retryCount = 0;
            }
        }

        // 1. Desserializar a mensagem original
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        var baseType = typeof(BaseQueue<>);
        BaseQueue<T>? originalMsg = JsonSerializer.Deserialize<BaseQueue<T>>(json);

        if (originalMsg != null)
        {
            // 2. Atualizar os valores desejados
            originalMsg.Retry = retryCount + 1;
            originalMsg.Erros ??= new();
            originalMsg.Erros.Add($"Tentativa {retryCount + 1} falhou em {DateTime.UtcNow:O}");
        }

        // 3. Serializar novamente
        var newBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(originalMsg));

        // 4. Criar novas propriedades com header atualizado
        var props = channel.CreateBasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            { "x-retry-count", (retryCount + 1).ToString() }
        };

        // 5. Publicar novamente
        if (retryCount >= generalSetting.RabbitMq.MaxRetries)
        {
            logger.LogWarning("Mensagem excedeu tentativas. Enviando para a fila morta.");
            channel.BasicPublish("", deadQueueName, props, newBody);
        }
        else
        {
            logger.LogInformation($"Reenviando mensagem para retry. Tentativa: {retryCount + 1}");
            channel.BasicPublish("", retryQueueName, props, newBody);
        }
    }

    public static QueueResponse<T> CreateResponse<T>(this BaseQueue<T> request, string errorMessage = "") where T : class
    {
        return new QueueResponse<T>(request.Mensagem, request.Retry + 1)
        {
            Erros = string.IsNullOrEmpty(errorMessage) ? new() : new List<string> { errorMessage }
        };
    }
}
