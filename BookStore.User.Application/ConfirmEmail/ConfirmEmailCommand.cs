using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.ConfirmEmail;

public record ConfirmEmailCommand(
Guid UserId ,
string Token
    ):IRequest<ConfirmEmailResult>;
