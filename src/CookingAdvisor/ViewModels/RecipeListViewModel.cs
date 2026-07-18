namespace CookingAdvisor.ViewModels;

// Full data for the recipe list page: the paged results, the echoed (normalized)
// filter so the form keeps its selected values, pagination metadata, and the
// category options for the filter dropdown.
public class RecipeListViewModel
{
    public RecipeFilterViewModel Filter { get; set; } = new();
    public IReadOnlyList<RecipeListItemViewModel> Recipes { get; set; } = [];
    public IReadOnlyList<CategoryOptionViewModel> Categories { get; set; } = [];

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public class CategoryOptionViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
