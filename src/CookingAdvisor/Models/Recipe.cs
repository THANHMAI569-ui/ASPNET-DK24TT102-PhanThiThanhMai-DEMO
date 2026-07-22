namespace CookingAdvisor.Models;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int Servings { get; set; }
    public int PrepMinutes { get; set; }
    public int CookMinutes { get; set; }
    public Difficulty Difficulty { get; set; }
    public Region Region { get; set; }
    public int CaloriesPerServing { get; set; }
    public string? ImageUrl { get; set; }
    public MealTypeFlags SuitableMealTypes { get; set; } =
        MealTypeFlags.Breakfast | MealTypeFlags.Lunch | MealTypeFlags.Dinner;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
