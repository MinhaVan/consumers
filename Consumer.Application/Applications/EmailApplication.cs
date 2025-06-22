using System.Net;
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
using Polly;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Consumer.Application.Applications;

public class EmailApplication(
    ILogger<EmailApplication> _logger,
    IValidator<EmailRequest> _validator,
    GeneralSetting generalSetting,
    IEmailRepository _emailRepository
) : IEmailApplication
{
    public async Task<QueueResponse<EmailRequest>> ExecuteAsync(BaseQueue<EmailRequest> request)
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
            request.Retry++;
            request.Erros = validation.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogError("Validação falhou: {Mensagem}. Erros: {Erros}",
                JsonSerializer.Serialize(request.Mensagem),
                string.Join(", ", request.Erros));
            return request.CreateResponse("Erro de validação: " + string.Join(", ", request.Erros));
        }

        try
        {
            var template = await _emailRepository.GetEmailTemplateByIdAsync(request.Mensagem.TipoEmail);
            await SendEmailAsync(request.Mensagem.Destinos, template.Assunto, GerarHtmlEmail(template.Corpo, request.Mensagem.Data));

            request.Erros = new();
            return request.CreateResponse();
        }
        catch (Exception ex)
        {
            request.Retry++;
            request.Erros = new List<string> { ex.Message };
            _logger.LogError("Erro ao tentar enviar o e-mail: {Mensagem}. Erros: {Erros}",
                JsonSerializer.Serialize(request.Mensagem),
                string.Join(", ", request.Erros));

            return request.CreateResponse("Erro ao enviar o e-mail: " + string.Join(", ", request.Erros));
        }
    }

    private string GerarHtmlEmail(string corpo, string data)
    {
        if (string.IsNullOrWhiteSpace(corpo) || string.IsNullOrWhiteSpace(data))
            return corpo;

        var variaveis = JsonSerializer.Deserialize<Dictionary<string, object>>(data);

        if (variaveis == null || variaveis.Count == 0)
            return corpo;

        foreach (var (chave, valor) in variaveis)
        {
            corpo = corpo.Replace($"{{{{{chave}}}}}", valor?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }


        return corpo;
    }

    private async Task SendEmailAsync(List<string> destinos, string titulo, string mensagemHtml)
    {
        var apiKey = generalSetting.SendGridKey;
        if (string.IsNullOrEmpty(apiKey))
            throw new Exception("API key do SendGrid não configurada no ambiente");

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("walterli.valadares@outlook.com", "Coopertrasmig");

        var tos = destinos.Select(email => new EmailAddress(email)).ToList();

        var plainTextContent = System.Text.RegularExpressions.Regex.Replace(mensagemHtml, "<.*?>", string.Empty);

        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
            from,
            tos,
            titulo,
            plainTextContent,
            mensagemHtml
        );

        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<Response>(r => r.StatusCode != HttpStatusCode.Accepted)
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(Math.Pow(10, attempt)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Tentativa {retryAttempt} falhou com status {outcome.Result?.StatusCode}. Nova tentativa em {timespan.TotalSeconds} segundos.");
                });

        var response = await policy.ExecuteAsync(() => client.SendEmailAsync(msg));

        if (response.StatusCode != HttpStatusCode.Accepted)
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"Erro ao enviar email: {response.StatusCode} - {errorBody}");
        }
    }

}