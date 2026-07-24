namespace CookingAdvisor.ViewModels;

public class ShoppingListDetailsViewModel
{
    public int Id { get; set; }
    public int MenuPlanId { get; set; }
    public required string MenuPlanName { get; set; }
    public IReadOnlyList<ShoppingListItemViewModel> Items { get; set; } = [];
}

public class ShoppingListItemViewModel
{
    public int Id { get; set; }
    public required string IngredientName { get; set; }
    public required string Group { get; set; }
    public decimal Quantity { get; set; }
    public required string Unit { get; set; }
    public bool IsPurchased { get; set; }
}
