using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Application.ConfirmEmail;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

// Предполагая, что AppDbContext где-то здесь

namespace BookStore.User.Infrastructure.services;

// Этот класс зависит от Identity (AppUser) и IConfiguration
public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(UserManager<AppUser> userManager, IConfiguration config, IRefreshTokenRepository refreshTokenRepository, ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _config = config;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }


    public async Task<AuthenticationResult?> AuthenticateAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) 
            return null;

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck) 
            return null;

        // 1. Получаем роли (если они используются в Identity)
        var userRoles = await _userManager.GetRolesAsync(appUser);
        var claims = BuildClaims(appUser, userRoles);
        
        // 2. Генерация JWT
        var accessToken = GenerateJwtToken(claims);
        // 2. Генерируем Refresh Token
        var refreshToken =_refreshTokenRepository.GenerateRefreshTokenAsync(appUser.Id);
       
        return new AuthenticationResult(accessToken, refreshToken,appUser.Id);
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
    

    public async Task<bool> CheckAppUserAsync(Guid userId)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;
        return true;
    }


    private List<Claim> BuildClaims(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            // Уникальный идентификатор пользователя (Id)
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        
            // Email пользователя
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        
            // standard JWT claim: Subject (обычно email или id)
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
        
            // Jti (JWT ID) — уникальный идентификатор самого токена. 
            // Помогает защититься от атак повторения (replay attacks).
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Добавляем все роли пользователя в claims
        // Это позволит работать атрибуту [Authorize(Roles = "admin")]
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    public async Task<string> GenerateAccessToken(Guid userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) 
            return String.Empty;
        var roles = await _userManager.GetRolesAsync(appUser);
        var accessToken = GenerateJwtToken(BuildClaims(appUser, roles));
        return accessToken;
    }
    
    private string GenerateJwtToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Key is not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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