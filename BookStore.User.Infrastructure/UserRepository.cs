using BookStore.User.Application;
using BookStore.User.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.User.Infrastructure;

public class UserRepository:IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.User?> GetByIdAsync(Guid id)
    {
        var userentity = await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == id);
        return userentity;
    }
}