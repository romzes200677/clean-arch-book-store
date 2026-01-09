using System.Security.Claims;

namespace BookStore.User.Application.Interfaces;

public interface ISecurityService
{
    public List<Claim> BuildClaims(Guid userId, string email, IList<string> roles);
    public string GenerateJwtToken(List<Claim> claims);
}