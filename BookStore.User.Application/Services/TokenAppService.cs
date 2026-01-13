using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Domain;

namespace BookStore.User.Application.Services;

public class TokenAppService  : ITokenAppService
{
    private readonly IRefreshTokenInterface _refreshTokenInterface;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;

    public TokenAppService(IRefreshTokenInterface refreshTokenInterface, IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenInterface = refreshTokenInterface;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthenticationResult> IssueTokensAsync(Guid userId)
    {
        var newRefreshTokenString =  _refreshTokenInterface.GenerateRefreshToken(userId);
        var newToken = RefreshToken.CreateToken(newRefreshTokenString, userId);
        await _refreshTokenRepository.AddTokenAsync(newToken);
        var accessToken = await _refreshTokenInterface.GenerateAccessToken(userId);
        
        if (accessToken != string.Empty && newRefreshTokenString != string.Empty)
        {
            return new AuthenticationResult(accessToken, newRefreshTokenString, userId);
        }
        throw new UnauthorizedAccessException("Access is denied");
    }
}