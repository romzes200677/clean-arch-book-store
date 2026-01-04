using BookStore.User.Application;
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
        var applicationAssembly = typeof(User.Application.AssemblyMarker).Assembly; 
    
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(applicationAssembly));


        // 2. DbContext
        // Если ты выбрал подход "Отдельный контекст на модуль"
        services.AddDbContext<UsersDbContext>(options => 
        {
            // Вместо UseSqlServer используем UseInMemoryDatabase.
            // "UsersDb" - это просто имя базы в оперативной памяти.
            // Если у тебя разные модули, можно давать им разные имена или одно общее.
            options.UseInMemoryDatabase("UsersDb"); 
        });


        // 3. Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, AppUserService>();
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        // Регистрация Minimal API для модуля
        // app.MapGroup("/api/users")
        //     .MapUsersApi(); // Метод-расширение, где лежат `app.MapPost...`
    }
}