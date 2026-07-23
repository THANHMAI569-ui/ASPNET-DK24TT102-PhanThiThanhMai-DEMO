using System.Security.Claims;
using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Controllers;

[Authorize]
public class MenuController(MenuPlannerService plannerService, AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new MenuIndexViewModel
        {
            Form = new MenuGenerateViewModel { WeekStartDate = DateOnly.FromDateTime(DateTime.Today) },
            Plans = await GetPlanSummariesAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(MenuGenerateViewModel form)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var plan = await plannerService.GenerateWeeklyPlanAsync(
                    CurrentUserId, form.Name, form.WeekStartDate, form.Region);
                return RedirectToAction(nameof(Details), new { id = plan.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        var model = new MenuIndexViewModel { Form = form, Plans = await GetPlanSummariesAsync() };
        return View(nameof(Index), model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await db.MenuPlans
            .Include(p => p.Items).ThenInclude(i => i.Recipe)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId);

        if (plan is null)
            return NotFound();

        var model = new MenuPlanDetailsViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            WeekStartDate = plan.WeekStartDate,
            Days = plan.Items
                .GroupBy(i => i.DayOfWeek)
                .OrderBy(g => g.Key)
                .Select(g => new MenuDayViewModel
                {
                    DayOfWeek = g.Key,
                    Meals = g.OrderBy(i => i.MealType)
                        .Select(i => new MenuMealViewModel
                        {
                            MealType = i.MealType,
                            RecipeId = i.RecipeId,
                            RecipeName = i.Recipe.Name,
                            ImageUrl = i.Recipe.ImageUrl,
                            CaloriesPerServing = i.Recipe.CaloriesPerServing
                        })
                        .ToList()
                })
                .ToList(),
            RecipeOptions = await db.Recipes
                .OrderBy(r => r.Name)
                .Select(r => new RecipeOptionViewModel { Id = r.Id, Name = r.Name })
                .ToListAsync()
        };
        return View(model);
    }

    // Change or fill in the dish for a single (day, meal) cell — creates the item if the
    // cell was previously emptied by DeleteItem.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateItem(int planId, int dayOfWeek, MealType mealType, int recipeId)
    {
        var plan = await db.MenuPlans.FirstOrDefaultAsync(p => p.Id == planId && p.UserId == CurrentUserId);
        if (plan is null)
            return NotFound();

        if (!await db.Recipes.AnyAsync(r => r.Id == recipeId))
            return NotFound();

        var item = await db.MenuPlanItems.FirstOrDefaultAsync(
            i => i.MenuPlanId == planId && i.DayOfWeek == dayOfWeek && i.MealType == mealType);

        if (item is null)
            db.MenuPlanItems.Add(new MenuPlanItem { MenuPlanId = planId, DayOfWeek = dayOfWeek, MealType = mealType, RecipeId = recipeId });
        else
            item.RecipeId = recipeId;

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(int planId, int dayOfWeek, MealType mealType)
    {
        var plan = await db.MenuPlans.FirstOrDefaultAsync(p => p.Id == planId && p.UserId == CurrentUserId);
        if (plan is null)
            return NotFound();

        var item = await db.MenuPlanItems.FirstOrDefaultAsync(
            i => i.MenuPlanId == planId && i.DayOfWeek == dayOfWeek && i.MealType == mealType);

        if (item is not null)
        {
            db.MenuPlanItems.Remove(item);
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = planId });
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private async Task<IReadOnlyList<MenuPlanSummaryViewModel>> GetPlanSummariesAsync() =>
        await db.MenuPlans
            .Where(p => p.UserId == CurrentUserId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new MenuPlanSummaryViewModel { Id = p.Id, Name = p.Name, WeekStartDate = p.WeekStartDate })
            .ToListAsync();
}
