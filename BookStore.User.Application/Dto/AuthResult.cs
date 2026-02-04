namespace BookStore.User.Application.Dto;

public abstract record BaseAuthResult();

public record SuccessAuthResult(
    string AccessToken, 
    string RefreshToken,
    Guid UserId):BaseAuthResult;

public record FailedAuthResult(string ErrorMessage):BaseAuthResult;
    
public record RequiredTwoFactorResult(
    Guid UserId
    ):BaseAuthResult;

public record TokenVerified(
):BaseAuthResult;

public record AuthCheckSuccess(Guid UserId, bool EnabledTwoFactor):BaseAuthResult;
    
    