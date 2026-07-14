using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Data;

// Application database context. Extends IdentityDbContext so ASP.NET Core Identity
// tables are managed here. Domain entities (Recipe, Ingredient, ...) are added in
// a later step (Phase 1b).
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
}
