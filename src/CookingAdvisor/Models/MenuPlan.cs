namespace CookingAdvisor.Models;

public class MenuPlan
{
    public int Id { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public required string Name { get; set; }
    public DateOnly WeekStartDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<MenuPlanItem> Items { get; set; } = new List<MenuPlanItem>();
}
