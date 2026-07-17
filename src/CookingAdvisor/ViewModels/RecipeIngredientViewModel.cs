namespace CookingAdvisor.ViewModels;

public class RecipeIngredientViewModel
{
    public required string Name { get; set; }
    public decimal Quantity { get; set; }
    public required string Unit { get; set; }
}
