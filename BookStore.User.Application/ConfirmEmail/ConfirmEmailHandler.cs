using MediatR;

namespace BookStore.User.Application.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailCommand,ConfirmEmailResult>
{
    private readonly IUserService  _userService;

    public ConfirmEmailHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ConfirmEmailAsync(request.UserId, request.Token);
        return result;
    }
}