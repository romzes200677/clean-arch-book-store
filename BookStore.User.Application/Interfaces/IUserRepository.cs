namespace BookStore.User.Application.Interfaces;

public interface IUserRepository
{
    Task<Domain.User?> GetByIdAsync(Guid id);
    Task SaveUser(Domain.User user);
}
