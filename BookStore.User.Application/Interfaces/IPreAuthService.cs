namespace BookStore.User.Application.Interfaces;
public record CheckAuthResult(Guid userId);

public interface IPreAuthService
{

    public Task<CheckAuthResult> CheckAuthData(string email, string password);
}