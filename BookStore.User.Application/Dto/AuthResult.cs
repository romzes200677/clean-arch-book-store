namespace BookStore.User.Application.Dto;

public abstract record BaseAuthResult();

public record SuccessAuthResult(
    string AccessToken, 
    string RefreshToken,
    Guid UserId):BaseAuthResult;
    
public record RequiredTwoFactorResult(
    string email,
    string Provider 
    ):BaseAuthResult;