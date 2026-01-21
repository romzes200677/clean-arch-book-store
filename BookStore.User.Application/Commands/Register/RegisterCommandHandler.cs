using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Utils;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Commands.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IPostAuthService  _postAuthService;
    private readonly IPreAuthService _preAuthService;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IPostAuthService postAuthService, IPreAuthService preAuthService, IDomainUserRepository domainUserRepository, IUnitOfWork unitOfWork)
    {
        _postAuthService = postAuthService;
        _preAuthService = preAuthService;
        _domainUserRepository = domainUserRepository;
        _unitOfWork = unitOfWork;
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
            await _preAuthService.SendConfirmEmail(request.Email);
            await _unitOfWork.CommitAsync(cancellationToken);
    }
}
