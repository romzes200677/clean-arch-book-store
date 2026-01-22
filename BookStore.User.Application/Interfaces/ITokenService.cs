using BookStore.User.Application.Dto;

namespace BookStore.User.Application.Interfaces;


public interface ITokenService
{
    public Task<SuccessAuthResult> IssueTokensAsync(Guid userId);
    public Task<string> GenerateTokenForEmail(Guid userId);
    public Task<(Guid userId, string email, string token)> GenerateTwoFaToken(Guid userId, string provider);
    public Task<string> GetActiveTokenProvider(Guid userId);
}