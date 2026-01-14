namespace SharedKernel.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(string message) : base(message,"validation_error") {}
    
    // Тот самый удобный конструктор
    public ValidationException(string entityName, object key) 
        : base($"Ошибка валидации в сущности '{entityName}' с ключом ({key}).", $"{entityName.ToLower()}_validation_error") { }
    
}