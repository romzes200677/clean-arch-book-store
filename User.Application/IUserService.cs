using User.Application.Login;

namespace User.Application;

public interface IUserService
{
    Task<AuthenticationResult?> AuthenticateAsync(string email, string password);
}
