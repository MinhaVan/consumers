using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Domain.Interfaces.Applications;

public interface IQueueMessageHandler<T>
{
    Task<QueueResponse<T>> HandleAsync(BaseQueue<T> mensagem);
}
