using Consumer.Domain.Enum;

namespace Consumer.Domain.Models;

public class Template
{
    public int Id { get; set; }
    public TipoNotificacaoEnum TipoNotificacao { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public string? Copias { get; set; } = string.Empty;
    public string? CopiasOcultas { get; set; } = string.Empty;
    public string? Anexos { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
}