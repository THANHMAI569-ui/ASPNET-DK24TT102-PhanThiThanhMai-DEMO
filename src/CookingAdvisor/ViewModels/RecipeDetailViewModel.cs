using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class RecipeDetailViewModel
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
    public required string CategoryName { get; set; }
    public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
    public bool IsFavorite { get; set; }

    public int TotalMinutes => PrepMinutes + CookMinutes;
}
