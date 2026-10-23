using Microsoft.AspNetCore.Identity;

namespace LoginSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}