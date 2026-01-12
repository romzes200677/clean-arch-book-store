using BookStore.User.Domain;

namespace BookStore.User.Application.Interfaces;

public interface IRefreshTokenRepository
{
    public Task SaveTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenAsync(string token);
}