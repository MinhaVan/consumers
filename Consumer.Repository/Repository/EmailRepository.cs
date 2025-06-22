using Consumer.Domain.Enum;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Consumer.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Repository.Repository;

public class EmailRepository(EmailContext _context) : IEmailRepository
{
    public async Task<EmailTemplate> GetEmailTemplateByIdAsync(TipoEmailEnum tipoEmail)
    {
        return await _context.EmailTemplates.FirstOrDefaultAsync(x => x.TipoEmail == tipoEmail);
    }
}