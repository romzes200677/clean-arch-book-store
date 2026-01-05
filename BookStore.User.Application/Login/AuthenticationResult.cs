namespace BookStore.User.Application.Login;

public record AuthenticationResult(
    string AccessToken, 
    string RefreshToken, // Добавляем
    Guid UserId);