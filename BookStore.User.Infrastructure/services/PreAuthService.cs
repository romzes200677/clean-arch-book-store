using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class PreAuthService: IPreAuthService
{
    
    private readonly UserManager<AppUser> _userManager;


    public PreAuthService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
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
        
        return new(userId: appUser.Id);
    }
}