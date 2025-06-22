namespace Consumer.Domain.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<string> GetToken();
}