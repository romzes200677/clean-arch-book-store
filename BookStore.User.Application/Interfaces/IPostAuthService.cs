namespace BookStore.User.Application.Interfaces.Features;

public record AuthenticationResult(
    string AccessToken, 
    string RefreshToken, // Добавляем
    Guid UserId);

public record ConfirmEmailResult(bool Success); 

public interface IPostAuthService
{

    Task<Guid> RegisterAsync(string email, string password);
    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
    public Task<(Guid userId, string token)?> PrepareResetAsync(string email);
    public Task<bool> ResetPassword(Guid userId, string token, string password);
}