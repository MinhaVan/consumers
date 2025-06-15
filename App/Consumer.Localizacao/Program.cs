using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using Consumer.Domain.Validators;
using Consumer.Localizacao.Consumers;
using Consumer.Domain.ViewModels.Localizacao;
using Consumer.Domain.Utils;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Application.Applications;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Repository.APIs;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                // Permite usar variáveis de ambiente
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Bind settings
                services.Configure<RabbitMqSettings>(context.Configuration.GetSection("RabbitMQ"));
                services.Configure<RoutesApiSettings>(context.Configuration.GetSection("RoutesApi"));

                services.AddHostedService<QueueConsumer>();
                services.AddSingleton<IValidator<EnviarLocalizacaoWebSocketRequest>, EnviarLocalizacaoWebSocketRequestValidator>();
                services.AddSingleton<ILocalizacaoApplication, LocalizacaoApplication>();
                services.AddSingleton<ILocalizacaoRepository, LocalizacaoRepository>();

                var routesApi = context.Configuration.GetSection("RoutesApi").Get<RoutesApiSettings>();
                services.AddHttpClient("routes-api", client =>
                {
                    client.BaseAddress = new Uri(routesApi.BaseUrl);
                    client.DefaultRequestHeaders.Add(routesApi.ApiKeyHeader, routesApi.ApiKeyValue);
                });
            })
            .Build();

        await host.RunAsync();
    }
}
