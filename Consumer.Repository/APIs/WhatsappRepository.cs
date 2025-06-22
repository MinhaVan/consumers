using System.Net.Http.Json;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Polly;

namespace Consumer.Repository.APIs;

public class WhatsappRepository : IWhatsappRepository
{
    private readonly HttpClient _httpClient;
    public WhatsappRepository(
        IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("whatsapp-api");
    }

    public async Task SendWhatsappAsync(MensagemWhatsapp mensagem)
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        var response = await policy.ExecuteAsync(() =>
            _httpClient.PostAsJsonAsync("send", mensagem));

        var result = await response.Content.ReadFromJsonAsync<BaseResponseHttp>();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(response.Content.ReadAsStringAsync().Result, null, response.StatusCode);
        }
    }
}