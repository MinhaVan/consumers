using System.Text;
using Consumer.Domain.Configuration;
using Consumer.Domain.Utils;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Application.Extensions;

public static class QueueExtensions
{
    public static void RetryMessage(this IModel channel, BasicDeliverEventArgs ea, GeneralSetting generalSetting, ILogger logger, string deadQueueName, string retryQueueName)
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

        var props = channel.CreateBasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            { "x-retry-count", (retryCount + 1).ToString() }
        };

        if (retryCount >= generalSetting.RabbitMq.MaxRetries)
        {
            logger.LogWarning("Mensagem excedeu tentativas. Enviando para a fila morta.");
            channel.BasicPublish("", deadQueueName, props, ea.Body);
        }
        else
        {
            logger.LogInformation($"Reenviando mensagem para retry. Tentativa: {retryCount + 1}");
            channel.BasicPublish("", retryQueueName, props, ea.Body);
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
