using BookStore.User.Application.Interfaces;
using BookStore.User.Domain;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.User.Infrastructure.repo;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;

    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task SaveTokenAsync(RefreshToken token)
    {
        var tokenEntry = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token.Token));
        tokenEntry.IsRevoked = true;
        _dbContext.RefreshTokens.Add(tokenEntry);
    }

    public  async  Task<RefreshToken> GetTokenAsync(string token)
    {
        var domainToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token));
        if (domainToken != null)
        {
            return new RefreshToken(domainToken.Token, domainToken.ExpiryDate, domainToken.UserId, domainToken.IsRevoked);
           
        }
        throw new Exception("Token not found");
    }

}