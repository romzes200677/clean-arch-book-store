using BookStore.User.Application.Interfaces.Features;

namespace BookStore.User.Application.Interfaces;

public interface ITokenAppService
{
    Task<AuthenticationResult> IssueTokensAsync(Guid userId);
}