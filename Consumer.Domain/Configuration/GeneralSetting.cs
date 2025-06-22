namespace Consumer.Domain.Configuration;

public class GeneralSetting
{
    public string SendGridKey { get; set; } = string.Empty;
    public RabbitMqSettings RabbitMq { get; set; } = new();
    public ApiWithKeySettings RoutesApi { get; set; } = new();
    public ApiSettings WhatsappAPI { get; set; } = new();
    public ApiWithKeySettings AuthAPI { get; set; } = new();
}