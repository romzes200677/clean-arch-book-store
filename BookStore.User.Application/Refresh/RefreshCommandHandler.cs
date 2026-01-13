using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Refresh;

public class RefdreshCommandHandler : IRequestHandler<RefreshCommand,AuthenticationResult?>
{
    private readonly IRefreshTokenInterface _refreshTokenInterface;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;

    private readonly IUnitOfWork _unitOfWork;

    public RefdreshCommandHandler(  IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, ISecurityService securityService, IRefreshTokenInterface refreshTokenInterface)
    {
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenInterface = refreshTokenInterface;
    }

    public async  Task<AuthenticationResult?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var oldToken = await _refreshTokenRepository.GetTokenAsync(request.RefreshToken);
        if(oldToken == null || !oldToken.IsActive() ) throw new Exception("cat not refresh token");
        oldToken.Revoke();
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _refreshTokenRepository.UpdateTokenAsync(oldToken);
            var newRefreshTokenString =  _refreshTokenInterface.GenerateRefreshToken(oldToken.UserId);
            var newToken = RefreshToken.CreateToken(newRefreshTokenString, oldToken.UserId);
            await _refreshTokenRepository.AddTokenAsync(newToken);
            var accessToken = await _refreshTokenInterface.GenerateAccessToken(oldToken.UserId);
            
            if (accessToken != string.Empty && newRefreshTokenString != string.Empty)
            {
                await _unitOfWork.CommitAsync(cancellationToken);
                return new AuthenticationResult(accessToken, newRefreshTokenString, oldToken.UserId);
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
            
        throw new Exception("can't refresh token");
    }
}
