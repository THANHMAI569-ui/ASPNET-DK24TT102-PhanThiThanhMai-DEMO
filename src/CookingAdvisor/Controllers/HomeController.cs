using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CookingAdvisor.Models;
using CookingAdvisor.Services;

namespace CookingAdvisor.Controllers;

public class HomeController(RecipeService recipeService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await recipeService.GetHomeAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
