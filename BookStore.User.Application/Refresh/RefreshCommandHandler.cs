using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
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
        var token = await _refreshTokenRepository.GetTokenAsync(request.RefreshToken);
        token.Revoke();
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _refreshTokenRepository.SaveTokenAsync(token);
            var newRefreshToken =  _refreshTokenInterface.GenerateRefreshToken(token.UserId);
            var accessToken = await _refreshTokenInterface.GenerateAccessToken(token.UserId);
            
            if (accessToken != string.Empty && newRefreshToken != string.Empty)
            {
                await _unitOfWork.CommitAsync(cancellationToken);
                return new AuthenticationResult(accessToken, newRefreshToken, token.UserId);
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
