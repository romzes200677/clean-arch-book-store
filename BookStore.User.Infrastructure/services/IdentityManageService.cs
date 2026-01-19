using BookStore.User.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public class IdentityManageService : IIdentityManageService
{
    private readonly UserManager<AppUser> _userManager;

    public IdentityManageService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task ChangePasswordAsync(Guid userId, string userName, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new NotFoundException("User not found");
        var result = await _userManager.ChangePasswordAsync(user, userName, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new ValidationException("Validation error", errors);
        }
    }
}