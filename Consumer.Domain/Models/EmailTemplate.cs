using Consumer.Domain.Enum;

namespace Consumer.Domain.Models;

public class EmailTemplate
{
    public int Id { get; set; }
    public TipoEmailEnum TipoEmail { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public List<string> Copias { get; set; } = new();
    public List<string> CopiasOcultas { get; set; } = new();
    public string? Anexos { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
}