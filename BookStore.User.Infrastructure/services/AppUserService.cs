using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Application;
using BookStore.User.Application.Login;
using BookStore.User.Domain;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// Предполагая, что AppDbContext где-то здесь

namespace BookStore.User.Infrastructure;

// Этот класс зависит от Identity (AppUser) и IConfiguration
public class AppUserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _dbContext; // Понадобится для получения данных доменного пользователя по ID

    public AppUserService(
        UserManager<AppUser> userManager, 
        IConfiguration config, 
        AppDbContext dbContext)
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
        var claims = BuildClaims(appUser, userRoles);
        
        // 2. Генерация JWT
        var token = GenerateJwtToken(claims);
        // 2. Генерируем Refresh Token
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString().Replace("-", ""), // Просто случайная строка
            UserId = appUser.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Живет долго
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
        return new AuthenticationResult(token, refreshToken.Token,appUser.Id);
    }

    public async Task RegisterAsync(string email, string password)
    {
        // Начинаем транзакцию через ваш основной DbContext
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
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

            // 3. Создаем бизнес-пользователя (UserProfile / User)
            // Мы используем тот же ID, что сгенерировал UserManager
            var businessUser = new Domain.User
            {
                Id = user.Id,
                Email = email,
                Role = Role.user,
                Name = email.Split('@')[0] // Пример заполнения имени
            };

            await _dbContext.Users.AddAsync(businessUser);
        
            // 4. Сохраняем бизнес-профиль
            await _dbContext.SaveChangesAsync();

            // 5. ПОДТВЕРЖДАЕМ транзакцию (только здесь данные реально зафиксируются в БД)
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // Если что-то пошло не так на любом этапе — отменяем всё
            await transaction.RollbackAsync();
            throw;
        }
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
    
    public async Task<AuthenticationResult?> RefreshTokenAsync(string tokenValue)
    {
        // 1. Ищем токен
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == tokenValue);

        if (storedToken == null || !storedToken.IsActive)
            return null;

        // 2. Ищем пользователя (через тот же контекст!)
        var appUser = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (appUser == null) return null;

        // 3. Ротация токенов в рамках одной транзакции
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            storedToken.IsRevoked = true; // Отзываем старый

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString("N"),
                UserId = appUser.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(appUser);
            var accessToken = GenerateJwtToken(BuildClaims(appUser, roles));

            await transaction.CommitAsync();
            return new AuthenticationResult(accessToken, newRefreshToken.Token, appUser.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
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
}