using BookStore.User.Application.Interfaces.Features;

namespace BookStore.User.Application.Interfaces;


public interface ITokenService
{
    public Task<AuthenticationResult> IssueTokensAsync(Guid userId);
    public Task<string> GenerateTokenForEmail(Guid userId);
}