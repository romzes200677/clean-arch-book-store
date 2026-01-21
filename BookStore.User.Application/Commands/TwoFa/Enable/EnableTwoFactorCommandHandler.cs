using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.Enable;

public class EnableTwoFactorCommandHandler: IRequestHandler<EnableTwoFactor>
{
    private readonly IPostAuthService  _postAuthService;

    public EnableTwoFactorCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task Handle(EnableTwoFactor request, CancellationToken cancellationToken)
    {
        await _postAuthService.EnableTwoFactor(request.UserId);
    }
}