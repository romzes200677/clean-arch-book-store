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


    public async Task UpdateTokenAsync(RefreshToken token)
    {
        var tokenEntry = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token.Token));
        if (tokenEntry == null) throw new ArgumentException("Token not fond");
        tokenEntry.IsRevoked = token.IsRevoked;
        _dbContext.RefreshTokens.Update(tokenEntry);
    }

    public  async  Task<RefreshToken?> GetTokenAsync(string token)
    {
        var domainToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token));
        if (domainToken != null)
        {
            return new RefreshToken(domainToken.Token, domainToken.ExpiryDate, domainToken.UserId, domainToken.IsRevoked);
           
        }
        return null;
    }

    public async Task AddTokenAsync(RefreshToken token)
    {
        var entity = new RefreshTokenEntity()
        {
            UserId = token.UserId,
            Token = token.Token,
            ExpiryDate = token.ExpiryDate,
            IsRevoked = token.IsRevoked,
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.RefreshTokens.AddAsync(entity);
    }
}