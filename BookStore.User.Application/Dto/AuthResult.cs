namespace BookStore.User.Application.Dto;

public abstract record BaseAuthResult();

public record SuccessAuthResult(
    string AccessToken, 
    string RefreshToken,
    Guid UserId):BaseAuthResult;
    
public record RequiredTwoFactorResult(
    Guid UserId,
    string Provider 
    ):BaseAuthResult;