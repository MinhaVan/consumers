using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using Consumer.Domain.Validators;
using Consumer.Domain.ViewModels.Localizacao;
using Consumer.Domain.Utils;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Application.Applications;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Repository.APIs;
using System.Text.Json;
using Consumer.Application.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Consumer.Domain.Configuration;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

                // Permite usar variáveis de ambiente
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Bind settings
                var generalSetting = context.Configuration
                    .GetSection("GeneralSetting")
                    .Get<GeneralSetting>() ?? throw new InvalidOperationException("GeneralSetting configuration is missing.");

                services.AddSingleton(generalSetting);

                services.AddSingleton<IValidator<EnviarLocalizacaoWebSocketRequest>, EnviarLocalizacaoWebSocketRequestValidator>();
                services.AddSingleton<ILocalizacaoApplication, LocalizacaoApplication>();
                services.AddSingleton<ILocalizacaoRepository, LocalizacaoRepository>();

                services.AddSingleton<IQueueMessageHandler<EnviarLocalizacaoWebSocketRequest>, LocalizacaoQueueHandler>();
                services.AddHostedService(sp =>
                    new GenericQueueConsumer<EnviarLocalizacaoWebSocketRequest>(
                        sp.GetRequiredService<ILogger<GenericQueueConsumer<EnviarLocalizacaoWebSocketRequest>>>(),
                        sp.GetRequiredService<GeneralSetting>(),
                        sp.GetRequiredService<IQueueMessageHandler<EnviarLocalizacaoWebSocketRequest>>(),
                        RabbitMqQueues.EnviarLocalizacao
                    ));

                services.AddHttpClient("routes-api", client =>
                {
                    client.BaseAddress = new Uri(generalSetting.RoutesApi.BaseUrl);
                    client.DefaultRequestHeaders.Add(generalSetting.RoutesApi.ApiKeyHeader, generalSetting.RoutesApi.ApiKeyValue);
                });
            })
            .Build();

        await host.RunAsync();
    }
}
