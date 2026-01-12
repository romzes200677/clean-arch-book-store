namespace BookStore.User.Application.Interfaces.Features;

public interface IRegisterInterface
{
    public Task<string> GenerateTokenForEmail(Guid userId);
    Task<Guid> RegisterAsync(string email, string password);
}