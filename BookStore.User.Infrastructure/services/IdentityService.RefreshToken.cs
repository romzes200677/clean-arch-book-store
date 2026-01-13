using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookStore.User.Infrastructure.services;

public partial class IdentityService : IRefreshTokenInterface
{

    private readonly ISecurityService  _securityService;

    public IdentityService(ISecurityService securityService, UserManager<AppUser> userManager, ILogger<IdentityService> logger)
    {
        _securityService = securityService;
        _userManager = userManager;
        _logger = logger;
    }

    public string GenerateRefreshToken(Guid userId)
    {
        return Guid.NewGuid().ToString("N");
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