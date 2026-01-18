using MediatR;

namespace BookStore.User.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
Guid UserId ,
string Token,
string Password
    ):IRequest<bool>;
