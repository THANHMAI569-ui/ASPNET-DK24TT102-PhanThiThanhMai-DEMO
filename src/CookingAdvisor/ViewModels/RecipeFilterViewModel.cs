using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

// Search/filter parameters for the recipe list page, bound from the query string.
// Every field is optional so any combination of filters (or none) is valid; an
// invalid enum/number simply binds to null and is ignored by the service.
public class RecipeFilterViewModel
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public Region? Region { get; set; }
    public Difficulty? Difficulty { get; set; }
    public int? MaxCookMinutes { get; set; }
    public int Page { get; set; } = 1;
}
