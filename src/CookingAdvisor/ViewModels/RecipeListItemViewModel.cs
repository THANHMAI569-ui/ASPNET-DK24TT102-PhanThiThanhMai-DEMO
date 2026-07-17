using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class RecipeListItemViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public int PrepMinutes { get; set; }
    public int CookMinutes { get; set; }
    public Difficulty Difficulty { get; set; }
    public Region Region { get; set; }

    public int TotalMinutes => PrepMinutes + CookMinutes;
}
