using System.Security.Claims;
using CookingAdvisor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingAdvisor.Controllers;

[Authorize]
public class FavoriteController(FavoriteService favoriteService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var recipes = await favoriteService.GetFavoritesAsync(CurrentUserId);
        return View(recipes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int recipeId, string? returnUrl)
    {
        var isFavorite = await favoriteService.ToggleFavoriteAsync(CurrentUserId, recipeId);
        TempData["Success"] = isFavorite ? "Đã thêm vào món yêu thích." : "Đã bỏ khỏi món yêu thích.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
