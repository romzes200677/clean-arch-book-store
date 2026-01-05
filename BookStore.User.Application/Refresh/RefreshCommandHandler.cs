using BookStore.User.Application.Login;
using MediatR;

namespace BookStore.User.Application.Refresh;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RefdreshCommandHandler : IRequestHandler<RefreshCommand,AuthenticationResult?>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public RefdreshCommandHandler(IUserService userService, IUserRepository userRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
    }

    public async  Task<AuthenticationResult?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return await _userService.RefreshTokenAsync(request.RefreshToken);
    }
}
