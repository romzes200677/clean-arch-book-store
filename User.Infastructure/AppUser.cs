using Microsoft.AspNetCore.Identity;

namespace User.Infastructure;

public class AppUser :IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}