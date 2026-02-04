using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.ResetPassword;

public class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand,bool>
{
    private readonly IPostAuthService  _postAuthService;

    public ResetPasswordCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _postAuthService.ResetPassword(request.email, request.Token,request.Password);
        return result;
    }
}