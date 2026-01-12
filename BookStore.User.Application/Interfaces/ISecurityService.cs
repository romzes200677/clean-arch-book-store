namespace BookStore.User.Application.Interfaces;

public interface ISecurityService
{
    public string GenerateJwtToken(Guid userId, string email, IList<string> roles);
}