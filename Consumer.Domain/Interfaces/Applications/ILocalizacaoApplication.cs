using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Domain.Interfaces.Applications;

public interface ILocalizacaoApplication
{
    Task<BaseQueue<EnviarLocalizacaoWebSocketRequest>> SaveLocalizacaoAsync(BaseQueue<EnviarLocalizacaoWebSocketRequest> request);
}