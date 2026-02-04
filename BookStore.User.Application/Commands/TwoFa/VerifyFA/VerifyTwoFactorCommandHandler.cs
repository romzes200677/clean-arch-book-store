using BookStore.User.Application.Dto;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.@new;
using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.VerifyFA;

public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, BaseAuthResult>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public VerifyTwoFactorCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<BaseAuthResult> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var provider = await _identityService.GetProvider(request.UserId);
        var result = await _identityService.ConfirmTwoFactorAsync(request.UserId, request.Token, provider);
        return result switch
        {
            FailedAuthResult fail => fail,
            TokenVerified success => await SuccessResultAsync(request.UserId),
            _ => throw new NotImplementedException()
        };
    }
    
    private async Task<BaseAuthResult> SuccessResultAsync(Guid UserId)
    {
        await _identityService.EnableTwoFactor(UserId);
        return await _tokenService.IssueTokensAsync(UserId);
    }
}