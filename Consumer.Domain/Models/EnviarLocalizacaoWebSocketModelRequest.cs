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
        Destino = new DestinoWebSocketModelRequest
        {
            Latitude = enviarLocalizacaoWebSocketRequest.Destino.Latitude,
            Longitude = enviarLocalizacaoWebSocketRequest.Destino.Longitude,
            ProximoAlunoId = enviarLocalizacaoWebSocketRequest.Destino.ProximoAlunoId
        };
    }

    public string TipoMensagem { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RotaId { get; set; }
    public DestinoWebSocketModelRequest Destino { get; set; }
}

public class DestinoWebSocketModelRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? ProximoAlunoId { get; set; }
}