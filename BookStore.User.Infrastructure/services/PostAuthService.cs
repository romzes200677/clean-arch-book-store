using System.Text;
using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public  class PostAuthService : IPostAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<PostAuthService> _logger;
    private readonly INotificationService _notification;
    private readonly ITokenService _tokenService;

    public PostAuthService(UserManager<AppUser> userManager, ILogger<PostAuthService> logger, INotificationService notification, ITokenService tokenService)
    {
        _userManager = userManager;
        _logger = logger;
        _notification = notification;
        _tokenService = tokenService;
    }

    
    
    public async Task<(string email,string token)?> PrepareResetAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        if (result is null)
            return null;
        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(result);
        var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
        return (result.Email,encodedToken);
    }
    public async Task<bool> ResetPassword(string email, string encodedToken, string password)
    {
        var user =  await _userManager.FindByEmailAsync(email);
        if (user == null) return false;
        string decodedToken;
        try
        {
            var bytedToken = WebEncoders.Base64UrlDecode(encodedToken);
            decodedToken = Encoding.UTF8.GetString(bytedToken);
        }
        catch (Exception)
        {
            throw new ValidationException("Некорректный формат токена");
        }
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(",",result.Errors.Select(x => x.Description));
            throw new ValidationException(errors);
        }
        return result.Succeeded;
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

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user == null) throw new NotFoundException("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return new UserProfileResponse(user.Id, user.Email, roles.ToList());
    }

    
}