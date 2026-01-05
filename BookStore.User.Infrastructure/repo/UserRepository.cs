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
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == id);
        return userEntity;
    }

    public async Task SaveRefreshToken(Guid userId, string token)
    {
        await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == userId);
    }
}