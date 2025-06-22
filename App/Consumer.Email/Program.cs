using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using Consumer.Domain.Validators;
using Consumer.Domain.ViewModels.Localizacao;
using Consumer.Domain.Utils;
using Consumer.Domain.Interfaces.Applications;
using Consumer.Application.Applications;
using System.Text.Json;
using Consumer.Application.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Consumer.Repository.Context;
using Consumer.Domain.Configuration;
using Consumer.Repository.Repository;
using Consumer.Domain.Interfaces.Repositories;

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

                var connection = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<EmailContext>(options =>
                    options.UseNpgsql(connection, b => b.MigrationsAssembly("Consumer.Email")));

                services.AddScoped<IValidator<EmailRequest>, EmailRequestValidator>();
                services.AddScoped<IEmailApplication, EmailApplication>();
                services.AddScoped<IEmailRepository, EmailRepository>();

                services.AddSingleton<IQueueMessageHandler<EmailRequest>, EmailQueueHandler>();
                services.AddHostedService(sp =>
                    new GenericQueueConsumer<EmailRequest>(
                        sp.GetRequiredService<ILogger<GenericQueueConsumer<EmailRequest>>>(),
                        sp.GetRequiredService<GeneralSetting>(),
                        sp.GetRequiredService<IQueueMessageHandler<EmailRequest>>(),
                        RabbitMqQueues.Email
                    ));
            })
            .Build();

        await host.RunAsync();
    }
}
