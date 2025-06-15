using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Consumer.Domain.Utils;
using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Consumer.Application.Applications;

public class LocalizacaoApplication(
    ILogger<LocalizacaoApplication> _logger,
    IValidator<EnviarLocalizacaoWebSocketRequest> _validator,
    ILocalizacaoRepository _localizacaoRepository,
    IOptions<RabbitMqSettings> rabbitMqSettings
) : ILocalizacaoApplication
{
    public async Task<BaseQueue<EnviarLocalizacaoWebSocketRequest>> SaveLocalizacaoAsync(BaseQueue<EnviarLocalizacaoWebSocketRequest> request)
    {
        if (request.Retry >= rabbitMqSettings.Value.MaxRetries)
        {
            _logger.LogError("Número máximo de tentativas atingido para a mensagem: {Mensagem}", request.Mensagem);
            return new BaseQueue<EnviarLocalizacaoWebSocketRequest>(request.Mensagem, rabbitMqSettings.Value.MaxRetries)
            {
                Erros = new List<string> { "Número máximo de tentativas atingido." }
            };
        }

        var validation = await _validator.ValidateAsync(request.Mensagem);
        if (!validation.IsValid)
        {
            _logger.LogError("Validação falhou para a mensagem: {Mensagem}. Erros: {Erros}", request.Mensagem, validation.Errors);

            request.Retry = request.Retry + 1;
            request.Erros = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return request;
        }

        var enviarLocalizacaoWebSocketModelRequest = new EnviarLocalizacaoWebSocketModelRequest(request.Mensagem);
        var saveLocalizacaoResponse = await _localizacaoRepository.SaveLocalizacaoAsync(enviarLocalizacaoWebSocketModelRequest);

        if (saveLocalizacaoResponse.Sucesso is false)
        {
            _logger.LogError("Erro ao salvar localização: {Mensagem}. Erros: {Erros}", request.Mensagem, saveLocalizacaoResponse.Erros);

            request.Retry = request.Retry + 1;
            request.Erros = saveLocalizacaoResponse.Erros.Select(e => e).ToList();
            return request;
        }

        request.Erros = new();
        return request;
    }
}