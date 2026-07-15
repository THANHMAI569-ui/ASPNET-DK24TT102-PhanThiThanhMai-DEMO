namespace CookingAdvisor.Models;

public class MenuPlanItem
{
    public int Id { get; set; }

    public int MenuPlanId { get; set; }
    public MenuPlan MenuPlan { get; set; } = null!;

    // 0-6, Sunday-Saturday
    public int DayOfWeek { get; set; }
    public MealType MealType { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
