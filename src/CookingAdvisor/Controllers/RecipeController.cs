using CookingAdvisor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CookingAdvisor.Controllers;

public class RecipeController(RecipeService recipeService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var recipes = await recipeService.GetRecipesAsync();
        return View(recipes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var recipe = await recipeService.GetRecipeDetailAsync(id);
        if (recipe is null)
            return NotFound();

        return View(recipe);
    }
}
