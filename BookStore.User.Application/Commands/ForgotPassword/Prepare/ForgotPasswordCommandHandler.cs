using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.Prepare;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IPostAuthService  _postAuthService;
    private readonly INotificationService  _notificationService;

    public ForgotPasswordCommandHandler(INotificationService notificationService, IPostAuthService postAuthService)
    {
        _notificationService = notificationService;
        _postAuthService = postAuthService;
    }


    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetData = await _postAuthService.PrepareResetAsync(request.Email);
        if (resetData != null)
        {
            await _notificationService.SendResetPassword(resetData.Value.email, resetData.Value.token);
        }
    }
}
