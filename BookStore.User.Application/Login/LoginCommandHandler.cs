using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly ILoginInterface _loginInterface;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenInterface _refreshTokenInterface;

    public LoginCommandHandler( 

        IUnitOfWork unitOfWork, 
        ILoginInterface loginInterface, IRefreshTokenInterface refreshTokenInterface)
    {
        _unitOfWork = unitOfWork;
        _loginInterface = loginInterface;
        _refreshTokenInterface = refreshTokenInterface;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _loginInterface.CheckAuthData(request.Email,request.Password);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var newRefreshToken =  _refreshTokenInterface.GenerateRefreshToken(authInfo.userId);
            var accessToken = await _refreshTokenInterface.GenerateAccessToken(authInfo.userId);
            if (accessToken != string.Empty && newRefreshToken != string.Empty)
            {
                await _unitOfWork.CommitAsync(cancellationToken);
                return new AuthenticationResult(accessToken, newRefreshToken, authInfo.userId);
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
