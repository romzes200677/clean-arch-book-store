using BookStore.User.Application.Interfaces.Features;

namespace BookStore.User.Application.Interfaces;
public record CheckAuthResult(Guid userId);

public interface IAuthService
{
    Task<AuthenticationResult> IssueTokensAsync(Guid userId);
    public Task<CheckAuthResult> CheckAuthData(string email, string password);
}