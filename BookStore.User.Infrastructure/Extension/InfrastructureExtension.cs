using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.@new;
using BookStore.User.Application.Interfaces.Repos;
using BookStore.User.Application.Interfaces.Utils;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.repo;
using BookStore.User.Infrastructure.seed;
using BookStore.User.Infrastructure.services;
using BookStore.User.Infrastructure.services.New;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        //<Business
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPostAuthService, PostAuthService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        // Регистрация UnitOfWork
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}