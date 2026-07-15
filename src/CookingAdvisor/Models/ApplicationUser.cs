using Microsoft.AspNetCore.Identity;

namespace CookingAdvisor.Models;

public class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }
    public string? FamilyName { get; set; }
}
