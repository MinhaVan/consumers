using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Consumer.Application.Extensions;
using Consumer.Domain.Configuration;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Consumer.Domain.Utils;
using Consumer.Domain.ViewModels;
using Consumer.Domain.ViewModels.Localizacao;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Consumer.Application.Applications;

[ExcludeFromCodeCoverage]
public class LocalizacaoApplication(
    ILogger<LocalizacaoApplication> _logger,
    IValidator<EnviarLocalizacaoWebSocketRequest> _validator,
    ILocalizacaoRepository _localizacaoRepository,
    GeneralSetting generalSetting
) : ILocalizacaoApplication
{
    public async Task<QueueResponse<EnviarLocalizacaoWebSocketRequest>> SaveLocalizacaoAsync(BaseQueue<EnviarLocalizacaoWebSocketRequest> request)
    {
        if (request is null || request.Mensagem is null)
        {
            _logger.LogError("Request ou Mensagem nula recebida.");
            return default!;
        }

        if (request.Retry >= generalSetting.RabbitMq.MaxRetries)
        {
            _logger.LogError("Máximo de tentativas atingido: {Mensagem}", JsonSerializer.Serialize(request.Mensagem));
            return request.CreateResponse("Número máximo de tentativas atingido.");
        }

        var validation = await _validator.ValidateAsync(request.Mensagem);
        if (!validation.IsValid)
        {
            request.Erros = validation.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogError("Validação falhou: {Mensagem}. Erros: {Erros}",
                JsonSerializer.Serialize(request.Mensagem),
                string.Join(", ", request.Erros));

            return request.CreateResponse("Erro de validação: " + string.Join(", ", request.Erros));
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

            return request.CreateResponse("Erro ao salvar localização: " + string.Join(", ", request.Erros));
        }

        request.Erros = new();
        return request.CreateResponse(string.Empty);
    }
}