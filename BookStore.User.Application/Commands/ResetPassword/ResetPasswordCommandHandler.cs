using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand,bool>
{
    private readonly IIdentityRecoveryService  _identityRecoveryService;

    public ResetPasswordCommandHandler(IIdentityRecoveryService identityRecoveryService)
    {
        _identityRecoveryService = identityRecoveryService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityRecoveryService.ResetPassword(request.UserId, request.Token,request.Password);
        return result;
    }
}