namespace CookingAdvisor.ViewModels;

public class IngredientListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}
