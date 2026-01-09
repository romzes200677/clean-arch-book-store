using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookStore.User.Infrastructure.services;

public class NofificationService : INofificationService
{
    private readonly string _baseUrl = "https://localhost:7116";
    private readonly UserManager<AppUser> _userManager;

    public NofificationService(string baseUrl, UserManager<AppUser> userManager)
    {
        _baseUrl = baseUrl;
        _userManager = userManager;
    }

    public Task NotifyAsync(Guid userId, string token)
    {
        Console.WriteLine($"{_baseUrl}/api/Auth/confirm-email?userId={userId}&token={token}");
        return Task.CompletedTask;
    }
}