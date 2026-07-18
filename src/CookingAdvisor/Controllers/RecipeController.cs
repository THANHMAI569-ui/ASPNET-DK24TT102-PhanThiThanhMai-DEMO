using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CookingAdvisor.Controllers;

public class RecipeController(RecipeService recipeService) : Controller
{
    public async Task<IActionResult> Index(RecipeFilterViewModel filter)
    {
        var model = await recipeService.SearchRecipesAsync(filter);
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var recipe = await recipeService.GetRecipeDetailAsync(id);
        if (recipe is null)
            return NotFound();

        return View(recipe);
    }
}
