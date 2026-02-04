using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.Enable;

public record EnableTwoFactorCommand
(  Guid UserId
    ):IRequest;