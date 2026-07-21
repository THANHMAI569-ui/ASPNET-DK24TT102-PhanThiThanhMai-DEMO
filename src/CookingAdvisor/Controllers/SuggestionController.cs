using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CookingAdvisor.Controllers;

public class SuggestionController(SuggestionService suggestionService) : Controller
{
    public async Task<IActionResult> Index(List<int> ingredientIds)
    {
        var selected = ingredientIds.Distinct().ToList();

        var model = new SuggestionViewModel
        {
            SelectedIngredientIds = selected,
            Ingredients = await suggestionService.GetIngredientOptionsAsync(),
            Results = selected.Count > 0 ? await suggestionService.SuggestAsync(selected) : []
        };

        return View(model);
    }
}
