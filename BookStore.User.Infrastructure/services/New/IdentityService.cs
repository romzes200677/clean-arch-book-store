using System.Text;
using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces.@new;
using BookStore.User.Application.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SharedKernel.BaseRsponse;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services.New;

public class IdentityService:IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICoderService  _coderService;
    

    public IdentityService(UserManager<AppUser> userManager, ICoderService coderService)
    {
        _userManager = userManager;
        _coderService = coderService;
    }

    public async Task<BaseAuthResult> CheckAuthData(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) 
            return new FailedAuthResult("Неверный email или пароль");

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck)
            return new FailedAuthResult("Неверный email или пароль");
        if(!await _userManager.IsEmailConfirmedAsync(appUser))
            return new FailedAuthResult("Не подтвержден email");
        
        return new AuthCheckSuccess(appUser.Id,appUser.TwoFactorEnabled);
    }

    public async Task<string> GetProvider(Guid UserId)
    {
        var  user = await _userManager.FindByIdAsync(UserId.ToString());
        var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
        if (!providers.Any())
        {
            throw new UnauthorizedException("Not authorized");
        }
        return providers.First();
    }

    public async  Task<BaseAuthResult> ConfirmTwoFactorAsync(Guid UserId, string code, string provider)
    {
        var appUser = await _userManager.FindByIdAsync(UserId.ToString());
        if (appUser == null) return new FailedAuthResult("User not found");
        
        var result = await _userManager.VerifyTwoFactorTokenAsync(appUser,provider,provider);
        if (!result)
        {
            return new FailedAuthResult("Token not verified");
        }
        return new TokenVerified();

    }

    public async Task<string>  GenerateTokenForEmail(Guid userId) 
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
        return encodedToken;
    }

    public async Task<(Guid userId,string email, string token)> GenerateTwoFaToken(Guid userId,string provider)
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var twoFaToken = await _userManager.GenerateTwoFactorTokenAsync(appUser, provider);
        return new(appUser.Id, appUser.Email,twoFaToken);
    }

    public async Task<BaseOperationResult<bool>> EnableTwoFactor(Guid userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) return  BaseOperationResult<bool>.Failure("User not found");
        if (!appUser.TwoFactorEnabled)
        {
            await _userManager.SetTwoFactorEnabledAsync(appUser, true);
            
        }
         return BaseOperationResult<bool>.Success(appUser.TwoFactorEnabled);
    }
    
    public async Task<BaseOperationResult<Guid>> RegisterAsync(string email, string password)
    {
        // 1. Создаем Identity аккаунт
        var user = new AppUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
        
            // Проверяем, если ошибка в том, что юзер уже есть
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                return  BaseOperationResult<Guid>.Failure($"Пользователь с email {email} уже зарегистрирован");
            }

            // Для остальных ошибок (пароль слишком простой и т.д.)
            return  BaseOperationResult<Guid>.Failure(error);
        }

        // 2. Назначаем роль в Identity
        await _userManager.AddToRoleAsync(user, "User");

        return BaseOperationResult<Guid>.Success(user.Id);
    }

    public async Task<BaseOperationResult<bool>> ConfirmEmailAsync(Guid userId, string tokenValue)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found");
        // 1. Принимаем закодированный токен из URL
        // 2. Декодируем его обратно в нормальный вид
        var originalToken = _coderService.DecodeToken(tokenValue);
        var resultConfirm = await _userManager.ConfirmEmailAsync(user, originalToken);
        if (!resultConfirm.Succeeded)
        {
            var error = string.Join(", ", resultConfirm.Errors.Select(e => e.Description));
            return  BaseOperationResult<bool>.Failure(error);
        }
        return BaseOperationResult<bool>.Success(true);
    }

    public Task<bool> ResetPassword(string email, string encodedToken, string password)
    {
        throw new NotImplementedException();
    }

    public Task<(string email, string token)?> PrepareResetAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task ChangePasswordAsync(Guid userId, string userName, string password)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}