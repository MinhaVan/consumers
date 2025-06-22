using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Application.Queues;

public class NotificacaoQueueHandler : IQueueMessageHandler<NotificacaoRequest>
{
    private readonly INotificacaoApplication _app;

    public NotificacaoQueueHandler(INotificacaoApplication app)
    {
        _app = app;
    }

    public Task<QueueResponse<NotificacaoRequest>> HandleAsync(BaseQueue<NotificacaoRequest> mensagem)
    {
        return _app.ExecuteAsync(mensagem);
    }
}
