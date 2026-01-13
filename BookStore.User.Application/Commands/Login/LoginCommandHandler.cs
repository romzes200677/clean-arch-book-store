using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly ILoginInterface _loginInterface;
    private readonly ITokenAppService  _tokenAppService;
    


    public LoginCommandHandler( 
        ILoginInterface loginInterface, ITokenAppService tokenAppService)
    {
        _loginInterface = loginInterface;
        _tokenAppService = tokenAppService;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _loginInterface.CheckAuthData(request.Email,request.Password);
        return await _tokenAppService.IssueTokensAsync(authInfo.userId);
    }
}
