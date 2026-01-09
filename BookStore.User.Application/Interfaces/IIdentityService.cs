namespace BookStore.User.Application.Interfaces;
public record ConfirmEmailResult(bool Success); 
public record AuthenticationResult(
    string AccessToken, 
    string RefreshToken, // Добавляем
    Guid UserId);
public interface IIdentityService
{
    public Task<Guid> CheckAuthData(string email, string password);
    Task<Guid> RegisterAsync(string email, string password);

    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
    public Task<string?> GetEmailUser(Guid userId);
    public Task<string> GenerateTokenForEmail(Guid userId);
    public Task<IList<string>> GetRoles(Guid userId);
    public Task<bool> CheckUser(Guid userId);
}