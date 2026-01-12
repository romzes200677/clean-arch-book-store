using BookStore.User.Application.Interfaces.Features;

namespace BookStore.User.Infrastructure.services;

public partial class IdentityService : ILoginInterface
{
    public async Task<CheckAuthResult> CheckAuthData(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) 
            throw new UnauthorizedAccessException("");

        var passwordCheck = await _userManager.CheckPasswordAsync(appUser, password);
        if (!passwordCheck) 
            throw new UnauthorizedAccessException("");
        var roles = await _userManager.GetRolesAsync(appUser);
        
        return new(
            roles: roles,
            userId: appUser.Id);
    }
}