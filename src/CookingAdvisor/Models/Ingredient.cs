namespace CookingAdvisor.Models;

public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Unit { get; set; }
    public required string Group { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
