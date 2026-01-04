namespace User.Application;

public interface IUserRepository
{
    Task<Domain.User?> GetByIdAsync(Guid id);
}
