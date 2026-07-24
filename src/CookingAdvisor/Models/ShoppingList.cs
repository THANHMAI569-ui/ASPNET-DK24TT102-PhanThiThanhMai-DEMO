namespace CookingAdvisor.Models;

public class ShoppingList
{
    public int Id { get; set; }

    public int MenuPlanId { get; set; }
    public MenuPlan MenuPlan { get; set; } = null!;

    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
