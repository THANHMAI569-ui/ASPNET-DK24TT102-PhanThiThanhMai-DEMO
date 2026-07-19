using CookingAdvisor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.RecipeCount = await db.Recipes.CountAsync();
        ViewBag.IngredientCount = await db.Ingredients.CountAsync();
        ViewBag.CategoryCount = await db.Categories.CountAsync();
        return View();
    }
}
