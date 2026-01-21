namespace BookStore.User.Application.Interfaces;

public interface INofificationService
{
    Task ConfirmEmailAsync(Guid userId,string token);
    Task SendTwoFactorCode(Guid userId,string token);
    Task SendResetPassword(Guid userId,string token);
}