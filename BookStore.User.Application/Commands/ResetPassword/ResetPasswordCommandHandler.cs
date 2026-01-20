using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand,bool>
{
    private readonly IAccountService  _accountService;

    public ResetPasswordCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _accountService.ResetPassword(request.UserId, request.Token,request.Password);
        return result;
    }
}