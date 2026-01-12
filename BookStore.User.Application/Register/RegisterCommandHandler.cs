using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using BookStore.User.Domain;
using MediatR;

namespace BookStore.User.Application.Register;

// Команда должна принимать логин/пароль и возвращать результат аутентификации

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IRegisterInterface  _registerInterface;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly INofificationService _nofificationService;
    private readonly IUnitOfWork _unitOfWork;
    

    public RegisterCommandHandler(IDomainUserRepository domainUserRepository, INofificationService nofificationService, IUnitOfWork unitOfWork, IRegisterInterface registerInterface)
    {
        _domainUserRepository = domainUserRepository;
        _nofificationService = nofificationService;
        _unitOfWork = unitOfWork;
        _registerInterface = registerInterface;
    }

    public async  Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
    
        try {
            var userId = await _registerInterface.RegisterAsync(request.Email, request.Password);
            var businessUser = new Domain.User
            {
                Id = userId,
                Email = request.Email,
                Role = Role.user,
                Name = request.Email.Split('@')[0] // Пример заполнения имени
            };
            await _domainUserRepository.SaveUser(businessUser);
            var tokenForEmail = await _registerInterface.GenerateTokenForEmail(businessUser.Id);
            await _nofificationService.NotifyAsync(userId, tokenForEmail);
            await _unitOfWork.CommitAsync(cancellationToken);

        } 
        catch {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
