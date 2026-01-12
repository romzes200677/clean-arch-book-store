using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookStore.User.Infrastructure.services;

public partial class IdentityService : IRefreshTokenInterface
{
    public AppDbContext  _dbContext { get; }
    private readonly ISecurityService  _securityService;

    public IdentityService(UserManager<AppUser> userManager, ILogger<IdentityService> logger, AppDbContext dbContext, ISecurityService securityService)
    {
        _userManager = userManager;
        _logger = logger;
        _dbContext = dbContext;
        _securityService = securityService;
    }

    public string GenerateRefreshToken(Guid userId)
    {
        var newRefreshToken = new RefreshTokenEntity
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

    public async  Task<string> GenerateAccessToken(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user is null) throw new InvalidOperationException("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        var token =   _securityService.GenerateJwtToken(userId,user.Email, roles);
        return token;
        
        
    }
}