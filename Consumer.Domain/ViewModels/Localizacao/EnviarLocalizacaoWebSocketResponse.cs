namespace Consumer.Domain.ViewModels.Localizacao;

public class EnviarLocalizacaoWebSocketResponse
{
    public string Mensagem { get; set; }
    public bool Sucesso { get; set; }
    public string Erro { get; set; }

    public EnviarLocalizacaoWebSocketResponse(string mensagem, bool sucesso, string erro = null)
    {
        Mensagem = mensagem;
        Sucesso = sucesso;
        Erro = erro;
    }
}