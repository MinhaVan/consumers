using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;

namespace Consumer.Application.Queues;

[ExcludeFromCodeCoverage]
public class LocalizacaoQueueHandler : IQueueMessageHandler<EnviarLocalizacaoWebSocketRequest>
{
    private readonly ILocalizacaoApplication _app;

    public LocalizacaoQueueHandler(ILocalizacaoApplication app)
    {
        _app = app;
    }

    public async Task<QueueResponse<EnviarLocalizacaoWebSocketRequest>> HandleAsync(BaseQueue<EnviarLocalizacaoWebSocketRequest> mensagem)
    {
        return await _app.SaveLocalizacaoAsync(mensagem);
    }
}
