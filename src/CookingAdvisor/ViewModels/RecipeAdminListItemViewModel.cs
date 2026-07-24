using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class RecipeAdminListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Region Region { get; set; }
    public Difficulty Difficulty { get; set; }
    public int IngredientCount { get; set; }
    public int MenuPlanUsageCount { get; set; }
}
