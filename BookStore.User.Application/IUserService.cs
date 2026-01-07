using BookStore.User.Application.ConfirmEmail;
using BookStore.User.Application.Login;

namespace BookStore.User.Application;

public interface IUserService
{
    Task<AuthenticationResult?> AuthenticateAsync(string email, string password);
    Task RegisterAsync(string email, string password);
    Task<AuthenticationResult?> RefreshTokenAsync(string tokenValue);
    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
}
