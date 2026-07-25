using CookingAdvisor.Data;
using CookingAdvisor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

public class RecipeService(AppDbContext db)
{
    public const int PageSize = 9;

    public async Task<RecipeListViewModel> SearchRecipesAsync(RecipeFilterViewModel filter)
    {
        var keyword = filter.Keyword?.Trim();
        var categoryId = filter.CategoryId is > 0 ? filter.CategoryId : null;
        var maxCookMinutes = filter.MaxCookMinutes is > 0 ? filter.MaxCookMinutes : null;

        var query = db.Recipes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(r => r.Name.Contains(keyword));
        if (categoryId is { } catId)
            query = query.Where(r => r.CategoryId == catId);
        if (filter.Region is { } region)
            query = query.Where(r => r.Region == region);
        if (filter.Difficulty is { } difficulty)
            query = query.Where(r => r.Difficulty == difficulty);
        if (maxCookMinutes is { } maxCook)
            // "Thời gian nấu" shown on the list is prep + cook, so filter on the same total.
            query = query.Where(r => r.PrepMinutes + r.CookMinutes <= maxCook);

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        var page = filter.Page < 1 ? 1 : filter.Page;
        if (totalPages > 0 && page > totalPages)
            page = totalPages;

        var recipes = await query
            .OrderBy(r => r.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(r => new RecipeListItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.Category.Name,
                ImageUrl = r.ImageUrl,
                PrepMinutes = r.PrepMinutes,
                CookMinutes = r.CookMinutes,
                CaloriesPerServing = r.CaloriesPerServing,
                Difficulty = r.Difficulty,
                Region = r.Region
            })
            .ToListAsync();

        var categories = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return new RecipeListViewModel
        {
            Filter = new RecipeFilterViewModel
            {
                Keyword = keyword,
                CategoryId = categoryId,
                Region = filter.Region,
                Difficulty = filter.Difficulty,
                MaxCookMinutes = maxCookMinutes,
                Page = page
            },
            Recipes = recipes,
            Categories = categories,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>Counts, a handful of dishes and the category rail for the landing page.</summary>
    public async Task<HomeViewModel> GetHomeAsync(int featuredCount = 6)
    {
        var featured = await db.Recipes
            .OrderBy(r => r.Id)
            .Select(r => new RecipeListItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.Category.Name,
                ImageUrl = r.ImageUrl,
                PrepMinutes = r.PrepMinutes,
                CookMinutes = r.CookMinutes,
                CaloriesPerServing = r.CaloriesPerServing,
                Difficulty = r.Difficulty,
                Region = r.Region
            })
            .Take(featuredCount)
            .ToListAsync();

        var categories = await db.Categories
            .Where(c => c.Recipes.Any())
            .OrderByDescending(c => c.Recipes.Count)
            .Select(c => new HomeCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                RecipeCount = c.Recipes.Count
            })
            .ToListAsync();

        // Two for the hero collage plus one distinct photo for the bento cell,
        // so the same dish is not shown three times on the landing page.
        var heroImages = await db.Recipes
            .Where(r => r.ImageUrl != null && r.ImageUrl != "")
            .OrderBy(r => r.Id)
            .Select(r => r.ImageUrl!)
            .Take(3)
            .ToListAsync();

        return new HomeViewModel
        {
            RecipeCount = await db.Recipes.CountAsync(),
            IngredientCount = await db.Ingredients.CountAsync(),
            FeaturedRecipes = featured,
            Categories = categories,
            HeroImageUrls = heroImages
        };
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
