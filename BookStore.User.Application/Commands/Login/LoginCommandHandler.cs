using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.@new;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseAuthResult>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService  _tokenSerivce;
    private readonly INotificationService _notificationService;

    public LoginCommandHandler(IIdentityService identityService, ITokenService tokenSerivce, INotificationService notificationService)
    {
        _identityService = identityService;
        _tokenSerivce = tokenSerivce;
        _notificationService = notificationService;
    }


    public async Task<BaseAuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CheckAuthData(request.Email,request.Password);
        return result switch
        {
            AuthCheckSuccess info when info.EnabledTwoFactor =>
                await HandleTwoFactorAuth(info, request.Email),
            AuthCheckSuccess info =>
                await _tokenSerivce.IssueTokensAsync(info.UserId),
            _ => result
        };
    }

    private async Task<BaseAuthResult> HandleTwoFactorAuth(AuthCheckSuccess info, string email)
    {
        var provider = await _identityService.GetProvider(info.UserId);
        var tokenData = await _identityService.GenerateTwoFaToken(info.UserId, provider);
        await _notificationService.SendTwoFactorCode(email, tokenData.token);
        return new RequiredTwoFactorResult(info.UserId);
    }
}