using BookStore.User.Application.ConfirmEmail;
using BookStore.User.Application.Login;

namespace BookStore.User.Application.Interfaces;

public interface IIdentityService
{
    Task<AuthenticationResult?> AuthenticateAsync(string email, string password);
    Task<Guid> RegisterAsync(string email, string password);

    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
    Task<bool> ExistUserAsync(Guid userId);
    public Task<string> GenerateAccessToken(Guid userId);
    public Task<string> GenerateTokenForEmail(Guid userId);
}