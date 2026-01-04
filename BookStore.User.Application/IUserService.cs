using BookStore.User.Application.Login;

namespace BookStore.User.Application;

public interface IUserService
{
    Task<AuthenticationResult?> AuthenticateAsync(string email, string password);
    Task RegisterAsync(string email, string password);
}
