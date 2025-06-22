using Consumer.Domain.Enum;

namespace Consumer.Domain.ViewModels.Localizacao;

public class EmailRequest
{
    public TipoEmailEnum TipoEmail { get; set; }
    public string Data { get; set; } = string.Empty;
    public List<string> Destinos { get; set; } = new();
    public string Assunto { get; set; } = string.Empty;
}