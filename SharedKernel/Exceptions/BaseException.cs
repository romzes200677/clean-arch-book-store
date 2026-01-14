namespace SharedKernel.Exceptions;

public abstract class BaseException :Exception
{
    // Код ошибки (например, "user_not_found"), удобный для фронтенда
    public string ErrorCode { get; }

    protected BaseException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}