using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.Enable;

public class EnableTwoFactorCommandHandler: IRequestHandler<EnableTwoFactorCommand>
{
    private readonly IPostAuthService  _postAuthService;

    public EnableTwoFactorCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        await _postAuthService.EnableTwoFactor(request.UserId);
    }
}