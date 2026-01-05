namespace BookStore.User.Application;

public interface IUserRepository
{
    Task<Domain.User?> GetByIdAsync(Guid id);
    Task SaveRefreshToken(Guid userId, string token);
}
