namespace Consumer.Domain.ViewModels.Localizacao;

public class EnviarLocalizacaoWebSocketRequest
{
    public string TipoMensagem { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RotaId { get; set; }
    public DestinoWebSocketRequest Destino { get; set; }
}


public class DestinoWebSocketRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? ProximoAlunoId { get; set; }
}