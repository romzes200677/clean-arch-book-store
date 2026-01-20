using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailCommand,ConfirmEmailResult>
{
    private readonly IAccountService  _accountService;

    public ConfirmEmailHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _accountService.ConfirmEmailAsync(request.UserId, request.Token);
        return result;
    }
}