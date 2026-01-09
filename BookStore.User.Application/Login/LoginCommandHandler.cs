using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Login;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly ISecurityService _securityService;

    public LoginCommandHandler(IIdentityService identityService, IDomainUserRepository domainUserRepository, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, ISecurityService securityService)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _securityService = securityService;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.CheckAuthData(request.Email,request.Password);
       
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
                var newRefreshToken = _refreshTokenRepository.GenerateRefreshTokenAsync(userId);
                var email = await _identityService.GetEmailUser(userId);
                if (email != null)
                {
                    var roles = await _identityService.GetRoles(userId);
                    var claims = _securityService.BuildClaims(userId, email, roles);
                    var accessToken =_securityService.GenerateJwtToken(claims);
                    if (accessToken != string.Empty && newRefreshToken != string.Empty)
                    {
                        await _unitOfWork.CommitAsync(cancellationToken);
                        return new AuthenticationResult(accessToken, newRefreshToken, userId);
                    }
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
