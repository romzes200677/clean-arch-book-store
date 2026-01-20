using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IAccountService  _accountService;
    private readonly INofificationService  _nofificationService;

    public ForgotPasswordCommandHandler(INofificationService nofificationService, IAccountService accountService)
    {
        _nofificationService = nofificationService;
        _accountService = accountService;
    }


    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetData = await _accountService.PrepareResetAsync(request.Email);
        if (resetData != null)
        {
            await _nofificationService.NotifyAsync(resetData.Value.userId, resetData.Value.token);
        }
    }
}
