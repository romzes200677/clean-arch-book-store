using MediatR;

namespace BookStore.User.Application.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserService userService, IUserRepository userRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
    }

    public async  Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _userService.RegisterAsync(request.Email, request.Password);
    }
}
