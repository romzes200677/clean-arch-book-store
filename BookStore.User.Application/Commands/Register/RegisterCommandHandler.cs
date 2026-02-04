using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.@new;
using BookStore.User.Application.Interfaces.Utils;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Commands.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IPostAuthService postAuthService, IDomainUserRepository domainUserRepository, IUnitOfWork unitOfWork, INotificationService notificationService, IIdentityService identityService)
    {


        _domainUserRepository = domainUserRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _identityService = identityService;
    }

    public async  Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
            var userId = await _postAuthService.RegisterAsync(request.Email, request.Password);
            var businessUser = new Domain.User
            {
                Id = userId,
                Email = request.Email,
                Role = Role.user,
                Name = request.Email.Split('@')[0] // Пример заполнения имени
            };
            await _domainUserRepository.SaveUser(businessUser);
            var emailToken = await _identityService.GenerateTokenForEmail(userId);
            await _notificationService.SendConfirmEmail(request.Email,emailToken);
            await _unitOfWork.CommitAsync(cancellationToken);
    }
}
