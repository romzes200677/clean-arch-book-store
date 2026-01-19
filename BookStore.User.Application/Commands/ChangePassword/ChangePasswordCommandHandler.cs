using BookStore.User.Application.Commands.ResetPassword;
using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler: IRequestHandler<ResetPasswordCommand,bool>
{
    private readonly IIdentityRecoveryService  _identityRecoveryService;

    public ChangePasswordCommandHandler(IIdentityRecoveryService identityRecoveryService)
    {
        _identityRecoveryService = identityRecoveryService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityRecoveryService.ResetPassword(request.UserId, request.Token,request.Password);
        return result;
    }
}