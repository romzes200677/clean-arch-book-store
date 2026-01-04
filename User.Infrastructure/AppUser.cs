using Microsoft.AspNetCore.Identity;

namespace User.Infrastructure;

    public class AppUser :IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
