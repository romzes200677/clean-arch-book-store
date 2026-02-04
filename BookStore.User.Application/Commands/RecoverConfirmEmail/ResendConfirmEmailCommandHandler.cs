using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.RecoverConfirmEmail;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand>
{
    private readonly IPreAuthService _preAuthService;

    public ResendConfirmEmailCommandHandler(IPreAuthService preAuthService)
    {
        _preAuthService = preAuthService;
    }


    public async  Task Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _preAuthService.SendConfirmEmail(request.email);
    }
}
