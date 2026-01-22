namespace BookStore.User.Application.Interfaces;

public interface INofificationService
{
    Task ConfirmEmailAsync(string email,string token);
    Task SendTwoFactorCode(string email,string token);
    public Task SendResetPassword(string email, string token, string newPassword);
}