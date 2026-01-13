using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailCommand,ConfirmEmailResult>
{
    private readonly IConfirmEmailInterface  _confirmEmail;

    public ConfirmEmailHandler(IConfirmEmailInterface confirmEmail)
    {
        _confirmEmail = confirmEmail;
    }

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _confirmEmail.ConfirmEmailAsync(request.UserId, request.Token);
        return result;
    }
}