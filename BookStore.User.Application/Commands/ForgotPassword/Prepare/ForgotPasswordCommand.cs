using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.Prepare;

public record ForgotPasswordCommand(
    string Email) : IRequest;
