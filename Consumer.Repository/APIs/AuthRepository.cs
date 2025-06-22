using System.Net.Http.Json;
using Consumer.Domain.Interfaces.Repositories;
using Consumer.Domain.Models;
using Consumer.Domain.ViewModels;
using Polly;

namespace Consumer.Repository.APIs;

public class AuthRepository : IAuthRepository
{
    private readonly HttpClient _httpClient;
    public AuthRepository(
        IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("auth-api");
    }

    public async Task<string> GetToken()
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        var response = await policy.ExecuteAsync(() =>
            _httpClient.GetAsync("v1/Token/Gerar/1"));

        var result = await response.Content.ReadFromJsonAsync<BaseResponseHttp<TokenResponse>>();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(response.Content.ReadAsStringAsync().Result, null, response.StatusCode);
        }

        return result.Data.AccessToken ?? throw new Exception("Token não retornado pela API de autenticação");
    }
}