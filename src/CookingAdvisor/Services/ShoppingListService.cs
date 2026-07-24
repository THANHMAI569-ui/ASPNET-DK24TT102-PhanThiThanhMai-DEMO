using CookingAdvisor.Data;
using CookingAdvisor.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

// Shopping list generator (PLAN.md §5.3): aggregates every RecipeIngredient across a
// MenuPlan's items by (IngredientId, Unit), summing quantity. Because a MenuPlan can
// repeat the same recipe across the week, SelectMany naturally counts that recipe's
// ingredients once per occurrence, so quantities scale with how many times a dish is
// cooked. Regenerating for the same plan replaces all prior items.
public class ShoppingListService(AppDbContext db)
{
    public async Task<ShoppingList> GenerateFromMenuPlanAsync(string userId, int menuPlanId)
    {
        var plan = await db.MenuPlans
            .Include(p => p.Items).ThenInclude(i => i.Recipe).ThenInclude(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(p => p.Id == menuPlanId && p.UserId == userId);

        if (plan is null)
            throw new InvalidOperationException("Menu plan not found.");

        var aggregated = plan.Items
            .SelectMany(i => i.Recipe.RecipeIngredients)
            .GroupBy(ri => (ri.IngredientId, ri.Unit))
            .Select(g => new ShoppingListItem
            {
                IngredientId = g.Key.IngredientId,
                Unit = g.Key.Unit,
                Quantity = g.Sum(ri => ri.Quantity)
            })
            .ToList();

        var list = await db.ShoppingLists.Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.MenuPlanId == menuPlanId);

        if (list is null)
        {
            list = new ShoppingList { MenuPlanId = menuPlanId, UserId = userId, CreatedAt = DateTime.UtcNow };
            db.ShoppingLists.Add(list);
        }
        else
        {
            db.ShoppingListItems.RemoveRange(list.Items);
            list.Items.Clear();
            list.CreatedAt = DateTime.UtcNow;
        }

        foreach (var item in aggregated)
            list.Items.Add(item);

        await db.SaveChangesAsync();
        return list;
    }
}
