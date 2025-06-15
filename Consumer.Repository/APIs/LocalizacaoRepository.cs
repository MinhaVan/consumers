using System.Net.Http.Json;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Polly;

namespace Consumer.Repository.APIs;

public class LocalizacaoRepository : ILocalizacaoRepository
{
    private readonly HttpClient _httpClient;
    public LocalizacaoRepository(
        IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("routes-api");
    }

    public async Task<BaseResponseHttp> SaveLocalizacaoAsync(EnviarLocalizacaoWebSocketModelRequest request)
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(5, attempt)));

        var response = await policy.ExecuteAsync(() =>
            _httpClient.PostAsJsonAsync("v1/LocalizacaoTrajeto", request));

        var result = await response.Content.ReadFromJsonAsync<BaseResponseHttp>();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(string.Join(',', result?.Erros));
        }

        return result!;
    }
}