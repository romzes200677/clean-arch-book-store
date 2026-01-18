using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public  class LoginService : ILoginInterface
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser>  _signInManager;
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
            throw new UnauthorizedException("Неверный email или пароль");

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck)
            throw new UnauthorizedException("Неверный email или пароль");
        if(!await _userManager.IsEmailConfirmedAsync(appUser))
             throw new UnauthorizedException("Не подтвержден email");
        var roles = await _userManager.GetRolesAsync(appUser);
        
        return new(
            roles: roles,
            userId: appUser.Id);
    }
}