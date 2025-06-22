using Consumer.Domain.Enum;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Consumer.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Repository.Repository;

public class NotificacaoRepository(NotificacaoContext _context) : INotificacaoRepository
{
    public async Task<Template> GetNotificacaoTemplateAsync(TipoNotificacaoEnum tipoNotificacao)
    {
        return await _context.Templates.FirstOrDefaultAsync(x => x.TipoNotificacao == tipoNotificacao);
    }
}