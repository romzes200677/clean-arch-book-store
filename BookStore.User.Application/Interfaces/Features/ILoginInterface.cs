namespace BookStore.User.Application.Interfaces.Features;

public record CheckAuthResult(IList<string> roles, Guid userId);

public interface ILoginInterface
{
    public Task<CheckAuthResult> CheckAuthData(string email, string password);
}