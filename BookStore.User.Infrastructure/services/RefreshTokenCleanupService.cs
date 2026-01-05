using BookStore.User.Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookStore.User.Infrastructure.services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;
    // Интервал запуска (24 часа)
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24);

    public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Фоновая служба очистки токенов запущена.");

        // Используем PeriodicTimer (доступен с .NET 6+)
        using PeriodicTimer timer = new PeriodicTimer(_cleanupInterval);

        try
        {
            // Цикл будет выполняться, пока приложение не остановится
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanUpTokensAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Фоновая служба очистки токенов останавливается.");
        }
    }

    private async Task CleanUpTokensAsync(CancellationToken ct)
    {
        _logger.LogInformation("Начало процесса удаления просроченных токенов...");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Удаляем токены, которые либо просрочены, либо были отозваны более суток назад
            // (Отозванные оставляем на сутки для безопасности/аналитики, потом удаляем)
            var expiredTokens = context.RefreshTokens
                .Where(t => t.ExpiryDate < DateTime.UtcNow || (t.IsRevoked && t.CreatedAt < DateTime.UtcNow.AddDays(-1)));

            int count = await expiredTokens.ExecuteDeleteAsync(ct);

            _logger.LogInformation("Очистка завершена. Удалено токенов: {Count}", count);
        }
    }
}