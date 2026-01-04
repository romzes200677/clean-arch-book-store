using MediatR;

namespace User.Application.Login;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IUserService userService, IUserRepository userRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Аутентификация через инфраструктурный слой
        var authResult = await _userService.AuthenticateAsync(request.Email, request.Password);

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
        
        // Возвращаем результат, который включает токен и UserId
        return authResult;
    }
}
