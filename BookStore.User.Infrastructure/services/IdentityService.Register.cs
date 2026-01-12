using System.Text;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace BookStore.User.Infrastructure.services;

public partial class IdentityService : IRegisterInterface
{
    public async Task<Guid> RegisterAsync(string email, string password)
    {
        // 1. Создаем Identity аккаунт
        var user = new AppUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Identity Error: {error}");
        }

        // 2. Назначаем роль в Identity
        await _userManager.AddToRoleAsync(user, "User");

        return user.Id;
    }
    
    public async Task<string>  GenerateTokenForEmail(Guid userId) 
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null) throw new InvalidOperationException("User not found");
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
        return encodedToken;
    }
}