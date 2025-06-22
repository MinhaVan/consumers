namespace Consumer.Domain.Configuration;

public class RabbitMqSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int MaxRetries { get; set; }
}

public class ApiWithKeySettings : ApiSettings
{
    public string ApiKeyHeader { get; set; }
    public string ApiKeyValue { get; set; }
}

public class ApiSettings
{
    public string BaseUrl { get; set; }
}