using Consumer.Domain.Models;

namespace Consumer.Domain.Interfaces.Repositories;

public interface ILocalizacaoRepository
{
    Task<BaseResponseHttp> SaveLocalizacaoAsync(EnviarLocalizacaoWebSocketModelRequest request);
}