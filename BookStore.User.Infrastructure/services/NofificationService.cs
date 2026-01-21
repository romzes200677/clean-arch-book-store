using BookStore.User.Application.Interfaces;

namespace BookStore.User.Infrastructure.services;

public class NofificationService : INofificationService
{
    private readonly string _baseUrl = "https://localhost:7116";
    
    public Task ConfirmEmailAsync(Guid userId, string token)
    {
        Console.WriteLine($"{_baseUrl}/api/Auth/confirm-email?userId={userId}&token={token}");
        return Task.CompletedTask;
    }

    public Task SendTwoFactorCode(Guid userId, string token)
    {
        Console.WriteLine($"Sent to email {userId} virification code 2FA : {token}  ");
        return Task.CompletedTask;
    }

    public Task SendResetPassword(Guid userId, string token)
    {
        Console.WriteLine($"Sent to email {userId}  code : {token}  ");
        return Task.CompletedTask;
    }
}