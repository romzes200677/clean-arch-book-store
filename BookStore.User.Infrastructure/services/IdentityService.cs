using System.Text;
using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookStore.User.Infrastructure.services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<AppUser> userManager,
        IConfiguration config,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _config = config;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }


    public async Task<Guid> CheckAuthData(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) 
            throw new UnauthorizedAccessException("");

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck) 
            throw new UnauthorizedAccessException("");
        return appUser.Id;
    }

    public async Task<Guid> RegisterAsync(string email, string password)
    {
            // 1. Создаем Identity аккаунт
            var user = new AppUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Identity Error: {error}");
            }

            // 2. Назначаем роль в Identity
            await _userManager.AddToRoleAsync(user, "User");

            return user.Id;
    }
    
    public async Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");
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
    

    public async Task<string?> GetEmailUser(Guid userId)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");
        return user.Email;
    }
    
    public async Task<bool> CheckUser(Guid userId)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;
        return true;
    }
    
    public async Task<IList<string>> GetRoles(Guid userId)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    
    public async Task<string>  GenerateTokenForEmail(Guid userId) 
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
        return encodedToken;
    }
}