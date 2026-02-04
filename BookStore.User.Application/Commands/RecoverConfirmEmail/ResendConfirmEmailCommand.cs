using MediatR;

namespace BookStore.User.Application.Commands.RecoverConfirmEmail;

public record ResendConfirmEmailCommand(string email):IRequest;
