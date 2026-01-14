namespace SharedKernel.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) 
        : base(message, "resource_not_found") { }

    // Тот самый удобный конструктор
    public NotFoundException(string entityName, object key) 
        : base($"Сущность '{entityName}' с ключом ({key}) не найдена.", $"{entityName.ToLower()}_not_found") { }
}