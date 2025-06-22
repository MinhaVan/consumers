using System.Diagnostics.CodeAnalysis;

namespace Consumer.Domain.ViewModels;

[ExcludeFromCodeCoverage]
public class QueueResponse<T>
{
    public QueueResponse()
    { }

    public QueueResponse(T data, int retry)
    {
        Mensagem = data;
        Retry = retry;
    }

    public T Mensagem { get; set; }
    public int Retry { get; set; }
    public List<string> Erros { get; set; } = new();
    public bool Sucesso => !Erros.Any();
}