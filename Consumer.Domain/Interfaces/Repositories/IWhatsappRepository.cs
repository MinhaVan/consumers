using Consumer.Domain.Models;

namespace Consumer.Domain.Interfaces.Repositories;

public interface IWhatsappRepository
{
    Task SendWhatsappAsync(MensagemWhatsapp mensagem);
}