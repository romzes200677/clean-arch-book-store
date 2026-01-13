using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Behaviors;

// BookStore.User.Application/Behaviors/TransactionBehavior.cs

public class TransactionBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse> // Применяется только к запросам MediatR
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // 1. Проверяем, является ли это командой (изменяет данные)
        // Если это Query (просто чтение), транзакция обычно не нужна.
        if (request is not ICommand) // Нужно создать пустой интерфейс-маркер ICommand
        {
            return await next();
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 2. Вызываем следующий шаг (сам Хэндлер)
            var response = await next();

            // 3. Если хэндлер отработал без ошибок — фиксируем
            await _unitOfWork.CommitAsync(cancellationToken);

            return response;
        }
        catch (Exception)
        {
            // 4. Если в хэндлере упало исключение — откатываем всё
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw; // Пробрасываем ошибку дальше (в глобальный обработчик ошибок)
        }
    }
}