using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Commands.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IAccountService  _accountService;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly INofificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    

    public RegisterCommandHandler(
        IDomainUserRepository domainUserRepository,
        INofificationService notificationService,
        IUnitOfWork unitOfWork, 
        IAccountService accountService)
    {
        _domainUserRepository = domainUserRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _accountService = accountService;
    }

    public async  Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
            var userId = await _accountService.RegisterAsync(request.Email, request.Password);
            var businessUser = new Domain.User
            {
                Id = userId,
                Email = request.Email,
                Role = Role.user,
                Name = request.Email.Split('@')[0] // Пример заполнения имени
            };
            await _domainUserRepository.SaveUser(businessUser);
            var tokenForEmail = await _accountService.GenerateTokenForEmail(businessUser.Id);
            await _notificationService.NotifyAsync(userId, tokenForEmail);
            await _unitOfWork.CommitAsync(cancellationToken);
    }
}
