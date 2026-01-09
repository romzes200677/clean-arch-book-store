using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Login;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        // 1. Аутентификация через инфраструктурный слой
        var authResult = await _identityService.AuthenticateAsync(request.Email, request.Password);
        
        if (authResult == null)
        {
            throw new UnauthorizedAccessException("Authentication failed.");
        }
        
        // 2. Получаем доменного пользователя для claims (если роли или другие данные не были в AppUser)
        // В данном случае, в IAuthenticationService мы уже добавили роли из AppUser. 
        // Если бы нам нужны были данные из User.Domain.User, мы бы использовали:
        // var domainUser = await _userRepository.GetByIdAsync(authResult.UserId);
        var user = await _userRepository.GetByIdAsync(authResult.UserId);
        if (user == null) throw new UnauthorizedAccessException("User not found.");
        
        await _unitOfWork.CommitAsync(cancellationToken);
        // Возвращаем результат, который включает токен и UserId
        return authResult;
    }
}
