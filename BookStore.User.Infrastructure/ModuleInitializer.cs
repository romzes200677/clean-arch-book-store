using BookStore.User.Application;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.seed;
using BookStore.User.Infrastructure.services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Architecture;

namespace BookStore.User.Infrastructure;

public class UsersModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 1. MediatR Registration
        // Важно: Сканируем ИМЕННО сборку Application, где лежат Use Cases и Handlers.
        // Assembly.GetExecutingAssembly() здесь даст Infrastructure, а нам нужно Application.
        var applicationAssembly = typeof(AssemblyMarker).Assembly; 
    
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(applicationAssembly));


        // 2. База данных

        
        services.AddDbContext<AppDbContext>(options =>
        {
            // 3. Указываем EF Core использовать наше открытое соединение
            options.UseSqlite(configuration["ConnectionStrings:DefaultConnection"]);
        });
        ////
        
        // 3. Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, AppUserService>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        //Background
        services.AddHostedService<RefreshTokenCleanupService>();
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        // Регистрация Minimal API для модуля
        // app.MapGroup("/api/users")
        //     .MapUsersApi(); // Метод-расширение, где лежат `app.MapPost...`
    }
}