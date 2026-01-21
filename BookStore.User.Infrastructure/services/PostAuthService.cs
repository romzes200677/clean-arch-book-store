using System.Text;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public  class PostAuthService : IPostAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<PostAuthService> _logger;
    private readonly INofificationService _notification;
    private readonly ITokenService _tokenService;

    public PostAuthService(UserManager<AppUser> userManager, ILogger<PostAuthService> logger, INofificationService notification, ITokenService tokenService)
    {
        _userManager = userManager;
        _logger = logger;
        _notification = notification;
        _tokenService = tokenService;
    }

    public async Task<Guid> RegisterAsync(string email, string password)
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
                throw new ConflictException($"Пользователь с email {email} уже зарегистрирован");
            }

            // Для остальных ошибок (пароль слишком простой и т.д.)
            throw new ValidationException(error);
        }

        // 2. Назначаем роль в Identity
        await _userManager.AddToRoleAsync(user, "User");

        return user.Id;
    }
    
  
    public async Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found");
        // 1. Принимаем закодированный токен из URL
        // 2. Декодируем его обратно в нормальный вид
        byte[] decodedTokenBytes = WebEncoders.Base64UrlDecode(tokenValue);
        string originalToken = Encoding.UTF8.GetString(decodedTokenBytes);
        var resultConfirm = await _userManager.ConfirmEmailAsync(user, originalToken);
        if (!resultConfirm.Succeeded)
        {
            var error = string.Join(", ", resultConfirm.Errors.Select(e => e.Description));
            _logger.LogInformation(error);
            return new ConfirmEmailResult(false);
        }
        return new ConfirmEmailResult(true);
    }
    public async Task<(Guid userId,string token)?> PrepareResetAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        if (result is null)
            return null;
        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(result);
        var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
        return (result.Id,encodedToken);
    }
    public async Task<bool> ResetPassword(Guid userId, string token, string password)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        string decodedToken;
        try
        {
            var bytedToken = WebEncoders.Base64UrlDecode(token);
            decodedToken = Encoding.UTF8.GetString(bytedToken);
        }
        catch (Exception)
        {
            throw new ValidationException("Некорректный формат токена");
        }
        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(",",result.Errors.Select(x => x.Description));
            throw new ValidationException(errors);
        }
        return result.Succeeded;
    }
    public async Task ChangePasswordAsync(Guid userId, string userName, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new NotFoundException("User not found");
        var result = await _userManager.ChangePasswordAsync(user, userName, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new ValidationException("Validation error", errors);
        }
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user == null) throw new NotFoundException("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return new UserProfileResponse(user.Id, user.Email, roles.ToList());
    }

    public async Task EnableTwoFactor(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user == null) throw new NotFoundException("User not found");
        var tokenProvider = await _tokenService.GetActiveTokenProvider(user.Id);
        var token = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
        await _notification.SendTwoFactorCode(user.Id, token);
    }
}