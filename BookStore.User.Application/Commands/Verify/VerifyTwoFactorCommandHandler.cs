using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.Verify;

public class VerifyTwoFactorCommandHandler: IRequestHandler<VerifyTwoFactorCommand,string>
{
    private readonly IPostAuthService  _postAuthService;

    public VerifyTwoFactorCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task<string> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}