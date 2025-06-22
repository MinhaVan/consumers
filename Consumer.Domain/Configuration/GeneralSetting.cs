namespace Consumer.Domain.Configuration;

public class GeneralSetting
{
    public string SendGridKey { get; set; } = string.Empty;
    public RabbitMqSettings RabbitMq { get; set; } = new();
    public RoutesApiSettings RoutesApi { get; set; } = new();
}