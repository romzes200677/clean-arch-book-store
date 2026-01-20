using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.Refresh;

public class RefdreshCommandHandler : IRequestHandler<RefreshCommand,AuthenticationResult?>
{
    private readonly ITokenService  _tokenService;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;

    public RefdreshCommandHandler(ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async  Task<AuthenticationResult?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var oldToken = await _refreshTokenRepository.GetTokenAsync(request.RefreshToken);
        if(oldToken == null || !oldToken.IsActive() ) throw new Exception("cat not refresh token");
        oldToken.Revoke();
      
        await _refreshTokenRepository.UpdateTokenAsync(oldToken);
        return await _tokenService.IssueTokensAsync(oldToken.UserId);
    }
}
