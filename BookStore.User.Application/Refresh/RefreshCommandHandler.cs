using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Login;
using MediatR;

namespace BookStore.User.Application.Refresh;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RefdreshCommandHandler : IRequestHandler<RefreshCommand,AuthenticationResult?>
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly ISecurityService _securityService;
    private readonly IUnitOfWork _unitOfWork;

    public RefdreshCommandHandler(IIdentityService identityService,  IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, ISecurityService securityService)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _securityService = securityService;
    }

    public async  Task<AuthenticationResult?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        //ищем в репозитории рефреш токен
        //получаем из рефреш токена id iddentity юзера
        //проверяем что юзер существует
        //если юзер найден делаем ротацию токенов и генерим новый access и refresh token
        //иначе ошибка
        
        var userId = await _refreshTokenRepository.GetUserByTokenAsync(request.RefreshToken);
        
        var isExistUser = await _identityService.CheckUser(userId);
        if (isExistUser)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var isRevoked = await _refreshTokenRepository.SetInvalidToken(request.RefreshToken);
                if (isRevoked)
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
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
        
        throw new Exception("can't refresh token");
   
    }
}
