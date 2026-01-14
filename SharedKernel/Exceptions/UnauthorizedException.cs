namespace SharedKernel.Exceptions;

public class UnauthorizedException :BaseException
{
    public UnauthorizedException(string message) : base(message,"Unauthorized") {}
}