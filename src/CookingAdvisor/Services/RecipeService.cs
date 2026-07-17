using CookingAdvisor.Data;
using CookingAdvisor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

public class RecipeService(AppDbContext db)
{
    public async Task<IReadOnlyList<RecipeListItemViewModel>> GetRecipesAsync()
    {
        return await db.Recipes
            .Include(r => r.Category)
            .OrderBy(r => r.Name)
            .Select(r => new RecipeListItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.Category.Name,
                ImageUrl = r.ImageUrl,
                PrepMinutes = r.PrepMinutes,
                CookMinutes = r.CookMinutes,
                Difficulty = r.Difficulty,
                Region = r.Region
            })
            .ToListAsync();
    }

    public async Task<RecipeDetailViewModel?> GetRecipeDetailAsync(int id)
    {
        return await db.Recipes
            .Where(r => r.Id == id)
            .Select(r => new RecipeDetailViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Instructions = r.Instructions,
                Servings = r.Servings,
                PrepMinutes = r.PrepMinutes,
                CookMinutes = r.CookMinutes,
                Difficulty = r.Difficulty,
                Region = r.Region,
                CaloriesPerServing = r.CaloriesPerServing,
                ImageUrl = r.ImageUrl,
                CategoryName = r.Category.Name,
                Ingredients = r.RecipeIngredients
                    .OrderBy(ri => ri.Ingredient.Name)
                    .Select(ri => new RecipeIngredientViewModel
                    {
                        Name = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}
