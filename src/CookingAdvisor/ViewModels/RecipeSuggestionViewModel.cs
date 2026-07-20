namespace CookingAdvisor.ViewModels;

public class RecipeSuggestionViewModel
{
    public int RecipeId { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalIngredientCount { get; set; }
    public int MatchedCount { get; set; }
    public required IReadOnlyList<string> MissingIngredients { get; set; }

    public bool CanCookNow => MissingIngredients.Count == 0;
    public double Coverage => TotalIngredientCount == 0 ? 0 : (double)MatchedCount / TotalIngredientCount;
}
