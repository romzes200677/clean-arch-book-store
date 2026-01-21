using BookStore.User.Api.Dto;
using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class PreAuthService: IPreAuthService
{
    
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService  _tokenService;


    public PreAuthService(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
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
        var provider = await GetActiveTokenProvider(appUser);
        
        return new(appUser.Id,appUser.TwoFactorEnabled,provider);
    }
    
    public async Task<SuccessAuthResult> VerifyCode(Guid UserId, string Code)
    {
        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if(user == null) throw new NotFoundException("User not found");
        var provider = await GetActiveTokenProvider(user);
        if (user.TwoFactorEnabled)
        {
            var result = await _userManager.VerifyTwoFactorTokenAsync(user,provider,Code);
            if (!result)
            {
                throw new UnauthorizedException("Token not verified");
            }
        }
       
        return await _tokenService.IssueTokensAsync(user.Id);
    }

    private async Task<string> GetActiveTokenProvider(AppUser user)
    {
        var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
        if (!providers.Any())
        {
            throw new UnauthorizedException("Not authorized");
        }
        return providers.First();
    }
}