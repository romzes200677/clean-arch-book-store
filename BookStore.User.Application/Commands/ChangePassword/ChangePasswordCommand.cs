using MediatR;

namespace BookStore.User.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
Guid UserId ,
string UserName,
string Password
    ):IRequest;
