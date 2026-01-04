using BookStore.User.Application;
using Microsoft.EntityFrameworkCore;

namespace BookStore.User.Infrastructure;

public class UserRepository:IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.User?> GetByIdAsync(Guid id)
    {
        var userentity = await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == id);
        return userentity;
    }
}