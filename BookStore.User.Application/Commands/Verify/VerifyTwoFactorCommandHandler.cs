using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.Verify;

public class VerifyTwoFactorCommandHandler: IRequestHandler<VerifyTwoFactorCommand,string>
{
    private readonly IIdentityManageService _identityManageService;

    public VerifyTwoFactorCommandHandler(IIdentityManageService identityManageService)
    {
        _identityManageService = identityManageService;
    }

    public async Task<string> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}