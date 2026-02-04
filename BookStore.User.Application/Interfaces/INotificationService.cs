namespace BookStore.User.Application.Interfaces;

public interface INotificationService
{
    Task ConfirmEmailAsync(string email,string token);
    Task SendTwoFactorCode(string email,string token);
    public Task SendResetPassword(string email, string token);
    public Task SendConfirmEmail(string email, string token);
}