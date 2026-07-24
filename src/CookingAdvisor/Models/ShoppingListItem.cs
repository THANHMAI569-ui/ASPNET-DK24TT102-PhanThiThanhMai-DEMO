namespace CookingAdvisor.Models;

public class ShoppingListItem
{
    public int Id { get; set; }

    public int ShoppingListId { get; set; }
    public ShoppingList ShoppingList { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public required string Unit { get; set; }
    public bool IsPurchased { get; set; }
}
