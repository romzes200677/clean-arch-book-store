using System.Text;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public  class ConfirmEmailService : IConfirmEmailInterface
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ConfirmEmailService> _logger;

    public ConfirmEmailService(UserManager<AppUser> userManager, ILogger<ConfirmEmailService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    

    public async Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue)
    {
        var user =  await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found");
        // 1. Принимаем закодированный токен из URL
        // 2. Декодируем его обратно в нормальный вид
        byte[] decodedTokenBytes = WebEncoders.Base64UrlDecode(tokenValue);
        string originalToken = Encoding.UTF8.GetString(decodedTokenBytes);
        var resultConfirm = await _userManager.ConfirmEmailAsync(user, originalToken);
        if (!resultConfirm.Succeeded)
        {
            var error = string.Join(", ", resultConfirm.Errors.Select(e => e.Description));
            _logger.LogInformation(error);
            return new ConfirmEmailResult(false);
        }
        return new ConfirmEmailResult(true);
    }
}