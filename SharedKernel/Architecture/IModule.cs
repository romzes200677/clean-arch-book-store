

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Architecture;

public interface IModule
{
    // Каждый модуль сам регистрирует свои сервисы
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    
    // Опционально: для настройки Endpoint'ов (Minimal API)
    void ConfigureEndpoints(IEndpointRouteBuilder app);
}