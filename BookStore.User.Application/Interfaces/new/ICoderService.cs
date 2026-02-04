namespace BookStore.User.Application.Interfaces.@new;

public interface ICoderService
{
    public string DecodeToken(string encodedBase64String);
    public string EncodeToken(string token);
}   