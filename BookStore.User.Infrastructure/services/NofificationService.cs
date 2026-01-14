using BookStore.User.Application.Interfaces;

namespace BookStore.User.Infrastructure.services;

public class NofificationService : INofificationService
{
    private readonly string _baseUrl = "https://localhost:7116";
    
    public Task NotifyAsync(Guid userId, string token)
    {
        Console.WriteLine($"{_baseUrl}/api/Auth/confirm-email?userId={userId}&token={token}");
        return Task.CompletedTask;
    }
}