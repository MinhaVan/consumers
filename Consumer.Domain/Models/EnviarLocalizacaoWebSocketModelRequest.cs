using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Domain.Models;

public class EnviarLocalizacaoWebSocketModelRequest
{
    public EnviarLocalizacaoWebSocketModelRequest(EnviarLocalizacaoWebSocketRequest enviarLocalizacaoWebSocketRequest)
    {
        TipoMensagem = enviarLocalizacaoWebSocketRequest.TipoMensagem;
        Latitude = enviarLocalizacaoWebSocketRequest.Latitude;
        Longitude = enviarLocalizacaoWebSocketRequest.Longitude;
        RotaId = enviarLocalizacaoWebSocketRequest.RotaId;
        ProximoAlunoId = enviarLocalizacaoWebSocketRequest.ProximoAlunoId;
    }

    public string TipoMensagem { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RotaId { get; set; }
    public int? ProximoAlunoId { get; set; }
}