using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailCommand,ConfirmEmailResult>
{
    private readonly IIdentityService  _identityService;

    public ConfirmEmailHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmailAsync(request.UserId, request.Token);
        return result;
    }
}