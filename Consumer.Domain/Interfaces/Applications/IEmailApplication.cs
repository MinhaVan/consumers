using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Domain.Interfaces.Applications;

public interface IEmailApplication
{
    Task<QueueResponse<EmailRequest>> ExecuteAsync(BaseQueue<EmailRequest> request);
}