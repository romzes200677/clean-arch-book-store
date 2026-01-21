namespace BookStore.User.Api.Dto;

public abstract record BaseAuthResult();

public record SuccessAuthResult(
    string? AccessToken, 
    string? RefreshToken,
    Guid? UserId):BaseAuthResult;
    
public record RequiredTwoFactorResult(
    Guid? UserId,
    string TokenFa 
    ):BaseAuthResult;