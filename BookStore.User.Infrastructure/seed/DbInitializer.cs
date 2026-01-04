using BookStore.User.Infrastructure.data;
using Microsoft.AspNetCore.Identity;

namespace BookStore.User.Infrastructure.seed;

public interface IDbInitializer
{
    Task InitializeAsync();
}

public class DbInitializer : IDbInitializer
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly AppDbContext _context;

    public DbInitializer(RoleManager<IdentityRole<Guid>> roleManager, AppDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    public async Task InitializeAsync()
    {
       // ВАЖНО: Создаем таблицы в базе данных, если их еще нет
        await _context.Database.EnsureCreatedAsync();
        // 1. Создаем роли, если их нет
        var roles = new[] { "User", "Admin", "Store" };
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        // 2. Можно добавить тестового админа для разработки
        // ... логика создания дефолтного админа ...

        await _context.SaveChangesAsync();
    }
}