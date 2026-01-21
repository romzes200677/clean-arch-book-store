using BookStore.User.Api.Dto;
using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseAuthResult>
{
    private readonly IPreAuthService _preAuthSerivce;
    private readonly ITokenService  _tokenSerivce;
    private readonly INofificationService _nofificationService;

    public LoginCommandHandler(IPreAuthService preAuthSerivce, ITokenService tokenSerivce, INofificationService nofificationService)
    {
        _preAuthSerivce = preAuthSerivce;
        _tokenSerivce = tokenSerivce;
        _nofificationService = nofificationService;
    }

    public async Task<BaseAuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _preAuthSerivce.CheckAuthData(request.Email,request.Password);
        if (authInfo.twoFactorEnabled)
        {
            var result = await _tokenSerivce.GenerateTwoFaToken(authInfo.userId, authInfo.provider);
            await _nofificationService.NotifyAsync(authInfo.userId, result.TokenFa);
         
        }
        return await _tokenSerivce.IssueTokensAsync(authInfo.userId);
    }
}
