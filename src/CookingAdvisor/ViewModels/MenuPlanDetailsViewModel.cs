using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class MenuPlanDetailsViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateOnly WeekStartDate { get; set; }
    public IReadOnlyList<MenuDayViewModel> Days { get; set; } = [];
}

public class MenuDayViewModel
{
    // 0-6, offset from WeekStartDate
    public int DayOfWeek { get; set; }
    public IReadOnlyList<MenuMealViewModel> Meals { get; set; } = [];
}

public class MenuMealViewModel
{
    public MealType MealType { get; set; }
    public int RecipeId { get; set; }
    public required string RecipeName { get; set; }
    public string? ImageUrl { get; set; }
}
