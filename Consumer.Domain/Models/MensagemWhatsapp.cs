using System.Diagnostics.CodeAnalysis;

namespace Consumer.Domain.Models;

[ExcludeFromCodeCoverage]
public class MensagemWhatsapp
{
    public string Number { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}