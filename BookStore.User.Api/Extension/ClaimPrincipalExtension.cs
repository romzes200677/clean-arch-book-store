using System.Security.Claims;
using SharedKernel.Exceptions;

namespace BookStore.User.Api.Extension;

public static class ClaimPrincipalExtension
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userIdString is null) throw new NotFoundException("User not found");
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) throw new UnauthorizedException("User not found");
            return userId;
    }
}