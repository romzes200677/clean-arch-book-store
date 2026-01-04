namespace BookStore.User.Application.Login;

public record AuthenticationResult(string Token, Guid UserId);