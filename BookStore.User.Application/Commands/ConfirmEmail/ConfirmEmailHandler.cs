using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailCommand,ConfirmEmailResult>
{
    private readonly IPostAuthService  _postAuthService;

    public ConfirmEmailHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _postAuthService.ConfirmEmailAsync(request.UserId, request.Token);
        return result;
    }
}