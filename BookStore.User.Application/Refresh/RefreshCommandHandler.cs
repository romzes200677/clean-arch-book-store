using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Login;
using MediatR;

namespace BookStore.User.Application.Refresh;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RefdreshCommandHandler : IRequestHandler<RefreshCommand,AuthenticationResult?>
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefdreshCommandHandler(IIdentityService identityService,  IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async  Task<AuthenticationResult?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        //ищем в репозитории рефреш токен
        //получаем из рефреш токена id iddentity юзера
        //проверяем что юзер существует
        //если юзер найден делаем ротацию токенов и генерим новый access и refresh token
        //иначе ошибка
        
        var userId = await _refreshTokenRepository.GetUserByTokenAsync(request.RefreshToken);
        
        var isExistUser = await _identityService.ExistUserAsync(userId);
        if (isExistUser)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var isRevoked = await _refreshTokenRepository.SetInvalidToken(request.RefreshToken);
                var newRefreshToken = _refreshTokenRepository.GenerateRefreshTokenAsync(userId);
                var newAccessToken = await _identityService.GenerateAccessToken(userId);
                await _unitOfWork.CommitAsync(cancellationToken);
                return new AuthenticationResult(newAccessToken, newRefreshToken, userId);
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
