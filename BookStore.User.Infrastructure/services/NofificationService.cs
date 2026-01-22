using BookStore.User.Application.Interfaces;

namespace BookStore.User.Infrastructure.services;

public class NofificationService : INofificationService
{
    private readonly string _baseUrl = "https://localhost:7116";
    
    public Task ConfirmEmailAsync(string email, string token)
    {
        Console.WriteLine($"{_baseUrl}/api/Auth/confirm-email?userId={email}&token={token}");
        return Task.CompletedTask;
    }

    public Task SendTwoFactorCode(string email, string token)
    {
        Console.WriteLine($"Sent to email {email} virification code 2FA : {token}  ");
        return Task.CompletedTask;
    }

    public Task SendResetPassword(string email, string token)
    {
        Console.WriteLine($" Url для фронта : {_baseUrl}/api/Auth/recovery/reset-password?email={email}&Token={token}");
        Console.WriteLine($"[LOG]: Письмо отправлено на {email}");
        Console.WriteLine($"[LOG]: Token: {token}");
        return Task.CompletedTask;
    }
}