namespace CookingAdvisor.ViewModels;

public class MenuIndexViewModel
{
    public required MenuGenerateViewModel Form { get; set; }
    public IReadOnlyList<MenuPlanSummaryViewModel> Plans { get; set; } = [];
}

public class MenuPlanSummaryViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateOnly WeekStartDate { get; set; }
}
