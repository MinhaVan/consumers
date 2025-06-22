using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Domain.Interfaces.Applications;

public interface INotificacaoApplication
{
    Task<QueueResponse<NotificacaoRequest>> ExecuteAsync(BaseQueue<NotificacaoRequest> request);
}