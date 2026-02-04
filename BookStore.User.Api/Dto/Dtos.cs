using BookStore.User.Application.Commands.ChangePassword;
using BookStore.User.Application.Commands.ConfirmEmail;
using BookStore.User.Application.Commands.ForgotPassword.Prepare;
using BookStore.User.Application.Commands.ForgotPassword.ResetPassword;
using BookStore.User.Application.Commands.Login;
using BookStore.User.Application.Commands.RecoverConfirmEmail;
using BookStore.User.Application.Commands.Refresh;
using BookStore.User.Application.Commands.Register;
using BookStore.User.Application.Commands.TwoFa.Enable;
using BookStore.User.Application.Commands.TwoFa.VerifyFA;
using BookStore.User.Application.Dto;

namespace BookStore.User.Api.Dto;

public record LoginDto(
    string Email,
    string Password
)
{
    public LoginCommand ToCommand() => new(Email, Password);
};

public record RegisterDto(
    string Email,
    string Password
)
{
    public RegisterCommand ToCommand() => new(Email, Password);
}

public record RefreshDto(
    string RefreshToken
)
{
    public RefreshCommand ToCommand() => new(RefreshToken);
}

public record ConfirmEmailDto(
    Guid UserId,string Token
)
{
    public ConfirmEmailCommand ToCommand() => new(UserId, Token);
}

public class ForgotPasswordDto(string email)
{
    public ForgotPasswordCommand ToCommand() => new(email);
}

public record ResetPasswordDto(
    string Email,
    string Token,
    string Password
)
{
    public ResetPasswordCommand  ToCommand() => new(Email, Token, Password);
}

public record ChangePasswordDto(string OldPassword, string NewPassword)
{
    public ChangePasswordCommand  ToCommand(Guid userId) => new(userId, OldPassword,NewPassword);
};

public record VerifyTwoFactorDto(Guid UserId, string Token)
{
    public VerifyTwoFactorCommand  ToCommand() => new(UserId, Token);
    
}

public record EnableTwoFactorDto(Guid UserId)
{
    public EnableTwoFactorCommand ToCommand() => new(UserId);
}

public record ResendConfirmEmailDto(string Email)
{
    public ResendConfirmEmailCommand ToCommand() => new(Email);
};

public record AuthResponseDto(
    bool RequiresTwoFactor,
    string? AccessToken = null,
    string? RefreshToken = null,
    Guid? UserId = null
);
