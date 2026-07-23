using System.Security.Claims;
using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CookingAdvisor.Controllers;

public class RecipeController(RecipeService recipeService, FavoriteService favoriteService) : Controller
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

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            recipe.IsFavorite = await favoriteService.IsFavoriteAsync(userId, id);
        }

        return View(recipe);
    }
}
