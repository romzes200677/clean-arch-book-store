using BookStore.User.Application.Commands.ResetPassword;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler: IRequestHandler<ResetPasswordCommand,bool>
{
    private readonly IPostAuthService  _postAuthService;

    public ChangePasswordCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _postAuthService.ResetPassword(request.UserId, request.Token,request.Password);
        return result;
    }
}