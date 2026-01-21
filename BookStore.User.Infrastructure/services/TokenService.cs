using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Api.Dto;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Repos;
using BookStore.User.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class TokenService: ITokenService
{
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public TokenService(IRefreshTokenRepository refreshTokenRepository, UserManager<AppUser> userManager, IConfiguration config)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _config = config;
    }

    public async Task<SuccessAuthResult> IssueTokensAsync(Guid userId)
    {
        var newRefreshTokenString =  GenerateRefreshToken(userId);
        var newToken = RefreshToken.CreateToken(newRefreshTokenString, userId);
        await _refreshTokenRepository.AddTokenAsync(newToken);
        var accessToken = await GenerateAccessToken(userId);
        
        if (accessToken != string.Empty && newRefreshTokenString != string.Empty)
        {
            return new SuccessAuthResult(accessToken, newRefreshTokenString, userId);
        }
        throw new UnauthorizedAccessException("Access is denied");
    }
    public string GenerateRefreshToken(Guid userId)
    {
        return Guid.NewGuid().ToString("N");
    }
    public async  Task<string> GenerateAccessToken(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user is null) throw new NotFoundException("Пользователь не найден для генерации токена");
        var roles = await _userManager.GetRolesAsync(user);
        var token =   GenerateJwtToken(userId,user.Email, roles);
        return token;
    }
    
    public string GenerateJwtToken(Guid userId, string email, IList<string> roles)
    {
        var claims =   BuildClaims(userId, email, roles);
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
    
    private List<Claim> BuildClaims(Guid userId,string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            // Уникальный идентификатор пользователя (Id)
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        
            // Email пользователя
            new Claim(ClaimTypes.Email, email ?? string.Empty),
        
            // standard JWT claim: Subject (обычно email или id)
            new Claim(JwtRegisteredClaimNames.Sub, email ?? string.Empty),
        
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
    
    public async Task<string>  GenerateTokenForEmail(Guid userId) 
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
        return encodedToken;
    }

    public async Task<RequiredTwoFactorResult> GenerateTwoFaToken(Guid userId,string provider)
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var twoFaToken = await _userManager.GenerateTwoFactorTokenAsync(appUser, provider);
        return new(userId, twoFaToken);
    }
}