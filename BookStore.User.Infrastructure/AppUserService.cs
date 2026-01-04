using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Application;
using BookStore.User.Application.Login;
using BookStore.User.Domain;
using BookStore.User.Infrastructure.data;
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