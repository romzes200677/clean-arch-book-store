using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.Enable;

public record EnableTwoFactor
(  Guid UserId
    ):IRequest;