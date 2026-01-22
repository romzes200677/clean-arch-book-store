using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.ResetPassword;

public record ResetPasswordCommand(
    string email ,
    string Token,
    string Password
    ):IRequest<bool>;
