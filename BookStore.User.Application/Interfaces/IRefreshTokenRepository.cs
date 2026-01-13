using BookStore.User.Domain;

namespace BookStore.User.Application.Interfaces;

public interface IRefreshTokenRepository
{
    public Task UpdateTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetTokenAsync(string token);
    Task AddTokenAsync(RefreshToken token);
}