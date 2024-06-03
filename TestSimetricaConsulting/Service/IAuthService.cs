using Domain;

namespace TestSimetricaConsulting.Service
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        Task<string> RegisterUser(User user);
        Task<User> Authenticate(string username, string password);
    }
}
