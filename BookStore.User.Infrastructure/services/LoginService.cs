using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookStore.User.Infrastructure.services;

public  class LoginService : ILoginInterface
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<AppUser> userManager, ILogger<LoginService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<CheckAuthResult> CheckAuthData(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) 
            throw new UnauthorizedAccessException("");

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck) 
            throw new UnauthorizedAccessException("");
        var roles = await _userManager.GetRolesAsync(appUser);
        
        return new(
            roles: roles,
            userId: appUser.Id);
    }
}