using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

public class FavoriteService(AppDbContext db)
{
    public async Task<IReadOnlyList<FavoriteItemViewModel>> GetFavoritesAsync(string userId) =>
        await db.Favorites
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.Recipe.Name)
            .Select(f => new FavoriteItemViewModel
            {
                Id = f.RecipeId,
                Name = f.Recipe.Name,
                CategoryName = f.Recipe.Category.Name,
                ImageUrl = f.Recipe.ImageUrl,
                Region = f.Recipe.Region,
                Difficulty = f.Recipe.Difficulty
            })
            .ToListAsync();

    public Task<bool> IsFavoriteAsync(string userId, int recipeId) =>
        db.Favorites.AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);

    // Returns the new favorite state (true = added, false = removed), or null
    // when the recipe does not exist. Without that check a tampered recipeId
    // would only fail at SaveChanges with an FK violation, i.e. a 500 page.
    public async Task<bool?> ToggleFavoriteAsync(string userId, int recipeId)
    {
        var existing = await db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        if (existing is not null)
        {
            db.Favorites.Remove(existing);
            await db.SaveChangesAsync();
            return false;
        }

        if (!await db.Recipes.AnyAsync(r => r.Id == recipeId))
            return null;

        db.Favorites.Add(new Favorite { UserId = userId, RecipeId = recipeId });
        await db.SaveChangesAsync();
        return true;
    }
}
