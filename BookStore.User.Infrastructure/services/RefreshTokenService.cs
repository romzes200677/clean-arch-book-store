using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;

namespace BookStore.User.Infrastructure.services;

public class RefreshTokenService : IRefreshTokenInterface
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ISecurityService  _securityService;

    public RefreshTokenService(ISecurityService securityService, UserManager<AppUser> userManager)
    {
        _securityService = securityService;
        _userManager = userManager;
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