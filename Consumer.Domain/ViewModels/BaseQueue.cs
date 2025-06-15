namespace Consumer.Domain.ViewModels.Localizacao;

public class BaseQueue<T>
{
    public BaseQueue()
    { }

    public BaseQueue(T data, int retry)
    {
        Mensagem = data;
        Retry = retry;
    }

    public T Mensagem { get; set; }
    public int Retry { get; set; }
    public List<string> Erros { get; set; } = new();
    public bool Sucesso => !Erros.Any();
}