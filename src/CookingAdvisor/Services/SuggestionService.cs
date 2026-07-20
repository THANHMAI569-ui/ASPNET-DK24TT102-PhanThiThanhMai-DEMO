using CookingAdvisor.Data;
using CookingAdvisor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

// Ingredient-based suggestion algorithm (PLAN.md §5.1): for each recipe compute
// matched / missing / coverage against the user's owned ingredient set, then rank
// cookable-now first → coverage descending → fewer missing ingredients.
public class SuggestionService(AppDbContext db)
{
    public async Task<List<RecipeSuggestionViewModel>> SuggestAsync(IReadOnlyCollection<int> ownedIngredientIds)
    {
        var owned = ownedIngredientIds.ToHashSet();
        if (owned.Count == 0)
            return [];

        // Recipes sharing no ingredient with the owned set are not worth suggesting.
        var recipes = await db.Recipes
            .Where(r => r.RecipeIngredients.Any(ri => owned.Contains(ri.IngredientId)))
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.ImageUrl,
                Ingredients = r.RecipeIngredients
                    .Select(ri => new { ri.IngredientId, ri.Ingredient.Name })
                    .ToList()
            })
            .ToListAsync();

        return recipes
            .Select(r =>
            {
                var missing = r.Ingredients
                    .Where(i => !owned.Contains(i.IngredientId))
                    .Select(i => i.Name)
                    .OrderBy(name => name)
                    .ToList();

                return new RecipeSuggestionViewModel
                {
                    RecipeId = r.Id,
                    Name = r.Name,
                    ImageUrl = r.ImageUrl,
                    TotalIngredientCount = r.Ingredients.Count,
                    MatchedCount = r.Ingredients.Count - missing.Count,
                    MissingIngredients = missing
                };
            })
            .OrderByDescending(s => s.CanCookNow)
            .ThenByDescending(s => s.Coverage)
            .ThenBy(s => s.MissingIngredients.Count)
            .ThenBy(s => s.Name)
            .ToList();
    }
}
