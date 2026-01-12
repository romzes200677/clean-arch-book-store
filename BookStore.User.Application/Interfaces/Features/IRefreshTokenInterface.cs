namespace BookStore.User.Application.Interfaces.Features;
public interface IRefreshTokenInterface
{
   public string GenerateRefreshToken(Guid userId);
   public Task<string> GenerateAccessToken(Guid userId);
}