using System.Security.Claims;
using CookingAdvisor.Data;
using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Controllers;

[Authorize]
public class ShoppingListController(ShoppingListService shoppingListService, AppDbContext db) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(int menuPlanId)
    {
        try
        {
            var list = await shoppingListService.GenerateFromMenuPlanAsync(CurrentUserId, menuPlanId);
            return RedirectToAction(nameof(Details), new { id = list.Id });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var list = await db.ShoppingLists
            .Include(l => l.MenuPlan)
            .Include(l => l.Items).ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == CurrentUserId);

        if (list is null)
            return NotFound();

        var model = new ShoppingListDetailsViewModel
        {
            Id = list.Id,
            MenuPlanId = list.MenuPlanId,
            MenuPlanName = list.MenuPlan.Name,
            Items = list.Items
                .OrderBy(i => i.Ingredient.Group)
                .ThenBy(i => i.Ingredient.Name)
                .Select(i => new ShoppingListItemViewModel
                {
                    Id = i.Id,
                    IngredientName = i.Ingredient.Name,
                    Group = i.Ingredient.Group,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    IsPurchased = i.IsPurchased
                })
                .ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePurchased(int id, int itemId)
    {
        var item = await db.ShoppingListItems
            .Include(i => i.ShoppingList)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == id && i.ShoppingList.UserId == CurrentUserId);

        if (item is null)
            return NotFound();

        item.IsPurchased = !item.IsPurchased;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
