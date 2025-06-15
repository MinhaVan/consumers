using System.Text.Json;
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
        if (request is null || request.Mensagem is null)
        {
            _logger.LogError("Request ou Mensagem nula recebida.");
            return new BaseQueue<EnviarLocalizacaoWebSocketRequest>(default!, request?.Retry ?? 0)
            {
                Erros = new List<string> { "Mensagem inválida." }
            };
        }

        if (request.Retry >= rabbitMqSettings.Value.MaxRetries)
        {
            _logger.LogError("Máximo de tentativas atingido: {Mensagem}", JsonSerializer.Serialize(request.Mensagem));
            return new BaseQueue<EnviarLocalizacaoWebSocketRequest>(request.Mensagem, request.Retry)
            {
                Erros = new List<string> { "Número máximo de tentativas atingido." }
            };
        }

        var validation = await _validator.ValidateAsync(request.Mensagem);
        if (!validation.IsValid)
        {
            request.Retry++;
            request.Erros = validation.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogError("Validação falhou: {Mensagem}. Erros: {Erros}",
                JsonSerializer.Serialize(request.Mensagem),
                string.Join(", ", request.Erros));
            return request;
        }

        var model = new EnviarLocalizacaoWebSocketModelRequest(request.Mensagem);
        var response = await _localizacaoRepository.SaveLocalizacaoAsync(model);

        if (!response.Sucesso)
        {
            request.Retry++;
            request.Erros = response.Erros.ToList();
            _logger.LogError("Erro ao salvar localização: {Mensagem}. Erros: {Erros}",
                JsonSerializer.Serialize(request.Mensagem),
                string.Join(", ", request.Erros));
            return request;
        }

        request.Erros = new();
        return request;
    }

}