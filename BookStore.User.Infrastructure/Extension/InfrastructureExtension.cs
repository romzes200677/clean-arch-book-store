using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.repo;
using BookStore.User.Infrastructure.seed;
using BookStore.User.Infrastructure.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService = BookStore.User.Infrastructure.services.AuthService;

namespace BookStore.User.Infrastructure.Extension;

public static class InfrastructureExtensions // Имя класса обычно во множественном числе
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(config["ConnectionStrings:DefaultConnection"]);
        });

        // Repositories & Services
        services.AddScoped<IDomainUserRepository, DomainDomainUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<INofificationService, NofificationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IIdentityManageService, IdentityManageService>();

        // Регистрация UnitOfWork
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}