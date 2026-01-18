using System.Text;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace BookStore.User.Infrastructure.services;

public class ForgotPasswordService : IForgotPassword
{
    private readonly UserManager<AppUser> _userManager;

    public ForgotPasswordService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(Guid userId,string token)?> PrepareResetAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        if (result is null)
            return null;
        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(result);
        var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
        return (result.Id,encodedToken);
    }
}