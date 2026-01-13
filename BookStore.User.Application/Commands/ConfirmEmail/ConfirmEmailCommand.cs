using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(
Guid UserId ,
string Token
    ):IRequest<ConfirmEmailResult>;
