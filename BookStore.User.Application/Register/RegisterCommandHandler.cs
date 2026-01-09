using BookStore.User.Application.Interfaces;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly INofificationService _nofificationService;
    private readonly IUnitOfWork _unitOfWork;
    

    public RegisterCommandHandler(IIdentityService identityService, IUserRepository userRepository, INofificationService nofificationService, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _nofificationService = nofificationService;
        _unitOfWork = unitOfWork;
    }

    public async  Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
    
        try {
            var userId = await _identityService.RegisterAsync(request.Email, request.Password);
            var businessUser = new Domain.User
            {
                Id = userId,
                Email = request.Email,
                Role = Role.user,
                Name = request.Email.Split('@')[0] // Пример заполнения имени
            };
            await _userRepository.SaveUser(businessUser);
            var tokenForEmail = await _identityService.GenerateTokenForEmail(businessUser.Id);
            await _nofificationService.NotifyAsync(userId, tokenForEmail);
            await _unitOfWork.CommitAsync(cancellationToken);

        } 
        catch {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
