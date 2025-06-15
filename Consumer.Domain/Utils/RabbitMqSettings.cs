namespace Consumer.Domain.Utils;

public class RabbitMqSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
public class RoutesApiSettings
{
    public string BaseUrl { get; set; }
    public string ApiKeyHeader { get; set; }
    public string ApiKeyValue { get; set; }
}