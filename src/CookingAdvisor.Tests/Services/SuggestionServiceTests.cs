using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.Services;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Tests.Services;

public class SuggestionServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task SeedAsync(AppDbContext db, params Recipe[] recipes)
    {
        var category = new Category { Id = 1, Name = "Món chính" };
        db.Categories.Add(category);

        var ingredientIds = recipes
            .SelectMany(r => r.RecipeIngredients.Select(ri => ri.IngredientId))
            .Distinct();
        foreach (var id in ingredientIds)
            db.Ingredients.Add(new Ingredient { Id = id, Name = $"Ingredient {id}", Unit = "g", Group = "Khác" });

        foreach (var recipe in recipes)
        {
            recipe.CategoryId = category.Id;
            db.Recipes.Add(recipe);
        }

        await db.SaveChangesAsync();
    }

    private static Recipe BuildRecipe(int id, string name, params int[] ingredientIds)
    {
        var recipe = new Recipe { Id = id, Name = name, Servings = 4 };
        foreach (var ingredientId in ingredientIds)
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = id,
                IngredientId = ingredientId,
                Quantity = 100,
                Unit = "g"
            });
        return recipe;
    }

    [Fact]
    public async Task SuggestAsync_AllIngredientsOwned_IsCookableWithNoMissing()
    {
        await using var db = CreateDb();
        await SeedAsync(db, BuildRecipe(1, "Thịt kho trứng", 1, 2, 3));
        var service = new SuggestionService(db);

        var results = await service.SuggestAsync([1, 2, 3]);

        var suggestion = Assert.Single(results);
        Assert.Equal(1, suggestion.RecipeId);
        Assert.True(suggestion.CanCookNow);
        Assert.Empty(suggestion.MissingIngredients);
        Assert.Equal(3, suggestion.MatchedCount);
        Assert.Equal(3, suggestion.TotalIngredientCount);
        Assert.Equal(1.0, suggestion.Coverage);
    }

    [Fact]
    public async Task SuggestAsync_SomeIngredientsMissing_ListsMissingNamesAndCoverage()
    {
        await using var db = CreateDb();
        await SeedAsync(db, BuildRecipe(1, "Canh chua cá", 1, 2, 3, 4));
        var service = new SuggestionService(db);

        var results = await service.SuggestAsync([1, 2]);

        var suggestion = Assert.Single(results);
        Assert.False(suggestion.CanCookNow);
        Assert.Equal(2, suggestion.MatchedCount);
        Assert.Equal(4, suggestion.TotalIngredientCount);
        Assert.Equal(0.5, suggestion.Coverage);
        Assert.Equal(["Ingredient 3", "Ingredient 4"], suggestion.MissingIngredients);
    }

    [Fact]
    public async Task SuggestAsync_NoMatchedIngredient_RecipeIsExcluded()
    {
        await using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Phở bò", 1, 2),
            BuildRecipe(2, "Gỏi cuốn", 3, 4));
        var service = new SuggestionService(db);

        var results = await service.SuggestAsync([1, 2]);

        var suggestion = Assert.Single(results);
        Assert.Equal(1, suggestion.RecipeId);
    }

    [Fact]
    public async Task SuggestAsync_RanksCookableFirstThenCoverageThenFewerMissing()
    {
        await using var db = CreateDb();
        await SeedAsync(db,
            // Owned set below is {1, 2, 3}.
            BuildRecipe(1, "Coverage 0.75, missing 1", 1, 2, 3, 4),
            BuildRecipe(2, "Cookable", 1, 2),
            BuildRecipe(3, "Coverage 0.5, missing 2", 1, 2, 4, 5),
            BuildRecipe(4, "Coverage 0.5, missing 1", 1, 4),
            BuildRecipe(5, "Coverage 0.25, missing 3", 1, 4, 5, 6));
        var service = new SuggestionService(db);

        var results = await service.SuggestAsync([1, 2, 3]);

        Assert.Equal(
            ["Cookable", "Coverage 0.75, missing 1", "Coverage 0.5, missing 1", "Coverage 0.5, missing 2", "Coverage 0.25, missing 3"],
            results.Select(r => r.Name).ToList());
    }

    [Fact]
    public async Task SuggestAsync_EmptyIngredientSet_ReturnsEmpty()
    {
        await using var db = CreateDb();
        await SeedAsync(db, BuildRecipe(1, "Phở bò", 1, 2));
        var service = new SuggestionService(db);

        var results = await service.SuggestAsync([]);

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetIngredientOptionsAsync_ReturnsAllIngredientsOrderedByName()
    {
        await using var db = CreateDb();
        db.Ingredients.Add(new Ingredient { Id = 1, Name = "Tỏi", Unit = "g", Group = "Gia vị" });
        db.Ingredients.Add(new Ingredient { Id = 2, Name = "Cà chua", Unit = "g", Group = "Rau" });
        await db.SaveChangesAsync();
        var service = new SuggestionService(db);

        var options = await service.GetIngredientOptionsAsync();

        Assert.Equal(["Cà chua", "Tỏi"], options.Select(o => o.Name).ToList());
        Assert.Equal([2, 1], options.Select(o => o.Id).ToList());
    }
}
