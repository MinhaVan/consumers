using Consumer.Domain.Enum;
using Consumer.Domain.Models;

namespace Consumer.Domain.Interfaces.Repositories;

public interface INotificacaoRepository
{
    Task<Template> GetNotificacaoTemplateAsync(TipoNotificacaoEnum id);
}