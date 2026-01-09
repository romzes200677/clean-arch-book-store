namespace BookStore.User.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<bool> SetInvalidToken(string token);
    Task<Guid> GetUserByTokenAsync(string token);
    public string GenerateRefreshTokenAsync(Guid userId);
}