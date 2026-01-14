namespace SharedKernel.Exceptions;

public class ConflictException: BaseException
{
    protected ConflictException(string message) : base(message,"Conflict Exception")
    {
    }

    // Тот самый удобный конструктор
    public ConflictException(string entityName, object key) 
        : base($"Сущность '{entityName}' с ключом ({key}) уже существует.", $"{entityName.ToLower()}_conflict_error") { }
}