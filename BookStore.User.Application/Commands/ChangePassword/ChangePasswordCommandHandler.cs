using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler: IRequestHandler<ChangePasswordCommand>
{
    private readonly IPostAuthService  _postAuthService;

    public ChangePasswordCommandHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await _postAuthService.ChangePasswordAsync(request.UserId, request.OldPassword,request.NewPassword);
    }
}