namespace BookStore.User.Application.Interfaces.Features;
public record ConfirmEmailResult(bool Success); 
public record AuthenticationResult(
    string AccessToken, 
    string RefreshToken, // Добавляем
    Guid UserId);

public interface IConfirmEmailInterface
{
    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
}