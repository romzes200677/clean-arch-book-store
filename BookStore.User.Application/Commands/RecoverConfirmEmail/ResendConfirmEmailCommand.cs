using MediatR;

namespace BookStore.User.Application.Commands.RecoverConfirmEmail;

public record ResendConfirmEmail(string email):IRequest;
