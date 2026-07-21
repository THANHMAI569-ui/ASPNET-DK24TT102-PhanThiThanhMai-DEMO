namespace CookingAdvisor.ViewModels;

// Suggestion page: the full ingredient list for the autocomplete picker, the
// currently selected ids (echoed back so chips survive a GET submit), and the
// ranked suggestions once the user has picked at least one ingredient.
public class SuggestionViewModel
{
    public IReadOnlyList<int> SelectedIngredientIds { get; set; } = [];
    public IReadOnlyList<IngredientOptionViewModel> Ingredients { get; set; } = [];
    public IReadOnlyList<RecipeSuggestionViewModel> Results { get; set; } = [];

    public bool HasSearched => SelectedIngredientIds.Count > 0;
}
