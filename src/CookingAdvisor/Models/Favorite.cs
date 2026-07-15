namespace CookingAdvisor.Models;

// Join entity between ApplicationUser and Recipe; composite key (UserId, RecipeId)
// is configured in AppDbContext.OnModelCreating.
public class Favorite
{
    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
