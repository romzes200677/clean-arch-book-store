using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Application;
using BookStore.User.Application.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// Предполагая, что AppDbContext где-то здесь

namespace BookStore.User.Infrastructure;

// Этот класс зависит от Identity (AppUser) и IConfiguration
public class AppUserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;
    private readonly UsersDbContext _dbContext; // Понадобится для получения данных доменного пользователя по ID

    public AppUserService(
        UserManager<AppUser> userManager, 
        IConfiguration config, 
        UsersDbContext dbContext)
    {
        _userManager = userManager;
        _config = config;
        _dbContext = dbContext;
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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(ClaimTypes.Email, appUser.Email ?? string.Empty),
        };
        
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // 2. Генерация JWT
        var token = GenerateJwtToken(claims, appUser.Id);

        return new AuthenticationResult(token, appUser.Id);
    }

    private string GenerateJwtToken(List<Claim> claims, Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));
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
}