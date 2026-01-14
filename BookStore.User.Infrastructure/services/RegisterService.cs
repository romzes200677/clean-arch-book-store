using System.Text;
using BookStore.User.Application.Interfaces.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SharedKernel.Exceptions;

namespace BookStore.User.Infrastructure.services;

public  class RegisterService : IRegisterInterface
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Guid> RegisterAsync(string email, string password)
    {
        // 1. Создаем Identity аккаунт
        var user = new AppUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
        
            // Проверяем, если ошибка в том, что юзер уже есть
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                throw new ConflictException($"Пользователь с email {email} уже зарегистрирован");
            }

            // Для остальных ошибок (пароль слишком простой и т.д.)
            throw new ValidationException(error);
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