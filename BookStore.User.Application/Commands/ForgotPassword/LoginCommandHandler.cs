using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IForgotPassword _forgotPassword;
    private readonly ISecurityService  _securityService;
    private readonly INofificationService  _nofificationService;

    public ForgotPasswordCommandHandler(IForgotPassword forgotPassword, ISecurityService securityService, INofificationService nofificationService)
    {
        _forgotPassword = forgotPassword;
        _securityService = securityService;
        _nofificationService = nofificationService;
    }


    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetData = await _forgotPassword.PrepareResetAsync(request.Email);
        if (resetData != null)
        {
            await _nofificationService.NotifyAsync(resetData.Value.userId, resetData.Value.token);
        }
    }
}
