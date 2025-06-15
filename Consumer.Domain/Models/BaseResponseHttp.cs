namespace Consumer.Domain.Models;

public class BaseResponseHttp<T>
{
    public bool Sucesso { get; set; } = true;
    public T Data { get; set; }
    public string Mensagem { get; set; } = "Operação realizada com sucesso";
    public List<string> Erros { get; set; } = new List<string>();
}

public class BaseResponseHttp
{
    public bool Sucesso { get; set; } = true;
    public object? Data { get; set; }
    public string Mensagem { get; set; } = "Operação realizada com sucesso";
    public List<string> Erros { get; set; } = new List<string>();
}