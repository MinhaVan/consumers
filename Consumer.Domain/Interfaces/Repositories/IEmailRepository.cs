using Consumer.Domain.Enum;
using Consumer.Domain.Models;

namespace Consumer.Domain.Interfaces.Repositories;

public interface IEmailRepository
{
    Task<EmailTemplate> GetEmailTemplateByIdAsync(TipoEmailEnum id);
}