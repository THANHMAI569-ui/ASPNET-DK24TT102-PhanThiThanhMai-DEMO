namespace CookingAdvisor.Models;

// Join entity between Recipe and Ingredient; composite key (RecipeId, IngredientId)
// is configured in AppDbContext.OnModelCreating.
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public required string Unit { get; set; }
}
