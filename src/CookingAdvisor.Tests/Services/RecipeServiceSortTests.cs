using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.Services;
using CookingAdvisor.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Tests.Services;

public class RecipeServiceSortTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Recipe BuildRecipe(
        int id, string name, int prepMinutes, int cookMinutes, int calories) =>
        new()
        {
            Id = id,
            Name = name,
            CategoryId = 1,
            Servings = 4,
            PrepMinutes = prepMinutes,
            CookMinutes = cookMinutes,
            CaloriesPerServing = calories
        };

    private static async Task SeedAsync(AppDbContext db, params Recipe[] recipes)
    {
        db.Categories.Add(new Category { Id = 1, Name = "Món chính" });
        db.Recipes.AddRange(recipes);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Defaults_to_alphabetical_order()
    {
        using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Canh chua", 10, 10, 300),
            BuildRecipe(2, "Bánh xèo", 10, 10, 500),
            BuildRecipe(3, "Đậu hũ sốt", 10, 10, 200));

        var result = await new RecipeService(db).SearchRecipesAsync(new RecipeFilterViewModel());

        Assert.Equal(
            ["Bánh xèo", "Canh chua", "Đậu hũ sốt"],
            result.Recipes.Select(r => r.Name));
    }

    [Fact]
    public async Task Sorts_by_total_time_using_prep_plus_cook()
    {
        using var db = CreateDb();
        await SeedAsync(db,
            // Longest cook time but the smallest total, so sorting on CookMinutes
            // alone would put this last instead of first.
            BuildRecipe(1, "Món A", 0, 25, 100),
            BuildRecipe(2, "Món B", 20, 20, 100),
            BuildRecipe(3, "Món C", 40, 5, 100));

        var result = await new RecipeService(db).SearchRecipesAsync(
            new RecipeFilterViewModel { Sort = RecipeSortOrder.TimeAsc });

        Assert.Equal(["Món A", "Món B", "Món C"], result.Recipes.Select(r => r.Name));
    }

    [Theory]
    [InlineData(RecipeSortOrder.CaloriesAsc, new[] { 150, 400, 900 })]
    [InlineData(RecipeSortOrder.CaloriesDesc, new[] { 900, 400, 150 })]
    public async Task Sorts_by_calories_in_both_directions(
        RecipeSortOrder sort, int[] expected)
    {
        using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Món A", 10, 10, 400),
            BuildRecipe(2, "Món B", 10, 10, 900),
            BuildRecipe(3, "Món C", 10, 10, 150));

        var result = await new RecipeService(db).SearchRecipesAsync(
            new RecipeFilterViewModel { Sort = sort });

        Assert.Equal(expected, result.Recipes.Select(r => r.CaloriesPerServing));
    }

    [Fact]
    public async Task Breaks_ties_by_name_so_paging_cannot_repeat_a_dish()
    {
        using var db = CreateDb();
        // Every dish shares one calorie value, so the tie-break is the only thing
        // deciding the order. Seeded in reverse name order on purpose: if the
        // tie-break were missing, the provider would fall back to insertion order
        // and page 1 would start at "Món 12" instead of "Món 01".
        var recipes = Enumerable.Range(1, RecipeService.PageSize + 3)
            .Select(i => BuildRecipe(i, $"Món {i:D2}", 10, 10, 500))
            .Reverse()
            .ToArray();
        await SeedAsync(db, recipes);

        var service = new RecipeService(db);
        var firstPage = await service.SearchRecipesAsync(
            new RecipeFilterViewModel { Sort = RecipeSortOrder.CaloriesDesc, Page = 1 });
        var secondPage = await service.SearchRecipesAsync(
            new RecipeFilterViewModel { Sort = RecipeSortOrder.CaloriesDesc, Page = 2 });

        Assert.Equal(
            Enumerable.Range(1, RecipeService.PageSize).Select(i => $"Món {i:D2}"),
            firstPage.Recipes.Select(r => r.Name));

        var seen = firstPage.Recipes.Select(r => r.Id)
            .Concat(secondPage.Recipes.Select(r => r.Id))
            .ToList();

        Assert.Equal(seen.Count, seen.Distinct().Count());
        Assert.Equal(recipes.Length, seen.Count);
    }

    [Fact]
    public async Task Sort_survives_alongside_a_filter_and_is_echoed_back()
    {
        using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Canh chua", 10, 10, 300),
            BuildRecipe(2, "Canh bí", 10, 10, 150),
            BuildRecipe(3, "Bánh xèo", 10, 10, 900));

        var result = await new RecipeService(db).SearchRecipesAsync(new RecipeFilterViewModel
        {
            Keyword = "Canh",
            Sort = RecipeSortOrder.CaloriesDesc
        });

        Assert.Equal(["Canh chua", "Canh bí"], result.Recipes.Select(r => r.Name));
        // The view rebuilds its sort links from the echoed filter, so losing this
        // would silently reset the control to "Tên A-Z" after every search.
        Assert.Equal(RecipeSortOrder.CaloriesDesc, result.Filter.Sort);
    }
}
