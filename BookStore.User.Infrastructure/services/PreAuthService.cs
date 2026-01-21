using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class PreAuthService: IPreAuthService
{
    
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService  _tokenService;
    private readonly INofificationService _nofificationService;


    public PreAuthService(UserManager<AppUser> userManager, ITokenService tokenService, INofificationService nofificationService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _nofificationService = nofificationService;
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
        var provider = await _tokenService.GetActiveTokenProvider(appUser.Id);
        
        return new(appUser.Id,appUser.TwoFactorEnabled,provider);
    }
    
    public async Task<SuccessAuthResult> VerifyCode(Guid UserId, string Code)
    {
        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if(user == null) throw new NotFoundException("User not found");
        var provider = await _tokenService.GetActiveTokenProvider(user.Id);
        
        var result = await _userManager.VerifyTwoFactorTokenAsync(user,provider,Code);
        if (!result)
        {
            throw new UnauthorizedException("Token not verified");
        }

        if (!user.TwoFactorEnabled)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
        }
        
        return await _tokenService.IssueTokensAsync(user.Id);
    }

    public async Task SendConfirmEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null) throw new NotFoundException("User not found");
        var tokenForEmail = await _tokenService.GenerateTokenForEmail(user.Id);
        await _nofificationService.ConfirmEmailAsync(user.Id, tokenForEmail);
    }
}