using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.Prepare;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IPostAuthService  _postAuthService;
    private readonly INofificationService  _nofificationService;

    public ForgotPasswordCommandHandler(INofificationService nofificationService, IPostAuthService postAuthService)
    {
        _nofificationService = nofificationService;
        _postAuthService = postAuthService;
    }


    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetData = await _postAuthService.PrepareResetAsync(request.Email);
        if (resetData != null)
        {
            await _nofificationService.SendResetPassword(resetData.Value.email, resetData.Value.token,request.NewPassword);
        }
    }
}
