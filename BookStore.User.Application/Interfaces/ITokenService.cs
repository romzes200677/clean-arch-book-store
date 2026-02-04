using BookStore.User.Application.Dto;

namespace BookStore.User.Application.Interfaces;


public interface ITokenService
{
    public Task<BaseAuthResult> IssueTokensAsync(Guid userId);

}