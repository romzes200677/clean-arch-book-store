using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.VerifyFA;

public class VerifyTwoFactorCommandHandler: IRequestHandler<VerifyTwoFactorCommand,BaseAuthResult>
{
    private readonly IPreAuthService  _preAuthService;

    public VerifyTwoFactorCommandHandler(IPreAuthService preAuthService)
    {
        _preAuthService = preAuthService;
    }

    public async Task<BaseAuthResult> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        return await _preAuthService.VerifyCode(request.UserId, request.Token);
    }
}