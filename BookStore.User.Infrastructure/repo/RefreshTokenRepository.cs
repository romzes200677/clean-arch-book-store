using BookStore.User.Application.Interfaces;
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

    public async Task<bool> SetInvalidToken(string token)
    {
       
            var domainToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token));
            if (domainToken == null) throw new Exception("Token not found");
            domainToken.IsRevoked = true;
            return true;
    }

    public async  Task<Guid> GetUserByTokenAsync(string token)
    {
        var domainToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token));
        if (domainToken != null)
        {
            return domainToken.UserId;
        }
        throw new Exception("Token not found");
    }
    
    private  async  Task<RefreshToken?> GetTokenAsync(string token)
    {
        var domainToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(token));
        if (domainToken != null)
        {
            return domainToken;
        }
        throw new Exception("Token not found");
    }
    public string GenerateRefreshTokenAsync(Guid userId)
    {
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString("N"),
            UserId = userId,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(newRefreshToken);
        return newRefreshToken.Token;
    }
}