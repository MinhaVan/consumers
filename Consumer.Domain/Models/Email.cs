namespace Consumer.Domain.Models;

public class Email
{
    public int Id { get; set; }
    public string Destinatario { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public bool Enviado { get; set; }
    public string? Erro { get; set; }
}