using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IAuthService _authSerivce;

    public LoginCommandHandler(IAuthService authSerivce)
    {
        _authSerivce = authSerivce;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _authSerivce.CheckAuthData(request.Email,request.Password);
        return await _authSerivce.IssueTokensAsync(authInfo.userId);
    }
}
