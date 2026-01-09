namespace BookStore.User.Application.Interfaces;

public interface IDomainUserRepository
{
    Task<Domain.User?> GetByIdAsync(Guid id);
    Task SaveUser(Domain.User user);
}
