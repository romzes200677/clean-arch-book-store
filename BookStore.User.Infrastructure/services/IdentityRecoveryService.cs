using System.Text;
using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class IdentityRecoveryService : IIdentityRecoveryService
{
    private readonly UserManager<AppUser> _userManager;
    
    public IdentityRecoveryService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<bool> ResetPassword(Guid userId, string token, string password)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        string decodedToken;
        try
        {
            var bytedToken = WebEncoders.Base64UrlDecode(token);
            decodedToken = Encoding.UTF8.GetString(bytedToken);
        }
        catch (Exception)
        {
            throw new ValidationException("Некорректный формат токена");
        }
        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(",",result.Errors.Select(x => x.Description));
            throw new ValidationException(errors);
        }
        return result.Succeeded;
    }
    
}