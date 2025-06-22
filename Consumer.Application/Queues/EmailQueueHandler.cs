using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Application.Queues;

public class EmailQueueHandler : IQueueMessageHandler<EmailRequest>
{
    private readonly IEmailApplication _app;

    public EmailQueueHandler(IEmailApplication app)
    {
        _app = app;
    }

    public Task<QueueResponse<EmailRequest>> HandleAsync(BaseQueue<EmailRequest> mensagem)
    {
        return _app.ExecuteAsync(mensagem);
    }
}
