using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IPreAuthService _preAuthSerivce;
    private readonly ITokenService  _tokenSerivce;

    public LoginCommandHandler(IPreAuthService preAuthSerivce, ITokenService tokenSerivce)
    {
        _preAuthSerivce = preAuthSerivce;
        _tokenSerivce = tokenSerivce;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _preAuthSerivce.CheckAuthData(request.Email,request.Password);
        return await _tokenSerivce.IssueTokensAsync(authInfo.userId);
    }
}
