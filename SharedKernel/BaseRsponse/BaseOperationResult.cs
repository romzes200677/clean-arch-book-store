namespace SharedKernel.BaseRsponse;

public record BaseOperationResult<T> 
{
    public T? Value { get; }
    public string[] Errors { get;}
    public bool IsSuccess { get; }
    
    private BaseOperationResult(T? value, string[] errors, bool isSuccess)
    {
        Value = value;
        Errors = errors;
        IsSuccess = isSuccess;
    }
    
    public static BaseOperationResult<T> Success(T value) =>
        new(value,Array.Empty<string>(),true);
    public static BaseOperationResult<T> Failure(params string[] errors) =>
        new(default, errors, false);
}
