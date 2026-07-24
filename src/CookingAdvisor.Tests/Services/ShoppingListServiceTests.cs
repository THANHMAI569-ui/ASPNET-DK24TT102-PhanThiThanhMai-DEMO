using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.Services;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Tests.Services;

public class ShoppingListServiceTests
{
    private const string UserId = "user-1";
    private const string OtherUserId = "user-2";

    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Recipe BuildRecipe(
        int id, string name, params (int IngredientId, decimal Quantity, string Unit)[] ingredients) =>
        new()
        {
            Id = id,
            Name = name,
            Servings = 4,
            CategoryId = 1,
            RecipeIngredients = ingredients
                .Select(i => new RecipeIngredient { IngredientId = i.IngredientId, Quantity = i.Quantity, Unit = i.Unit })
                .ToList()
        };

    private static async Task<MenuPlan> SeedPlanAsync(
        AppDbContext db, string userId, IReadOnlyList<Recipe> recipes, params int[] recipeIdsPerSlot)
    {
        db.Categories.Add(new Category { Id = 1, Name = "Món chính" });
        db.Recipes.AddRange(recipes);

        var plan = new MenuPlan
        {
            UserId = userId,
            Name = "Thực đơn tuần",
            WeekStartDate = new DateOnly(2026, 7, 27),
            CreatedAt = DateTime.UtcNow
        };
        db.MenuPlans.Add(plan);
        await db.SaveChangesAsync();

        for (var i = 0; i < recipeIdsPerSlot.Length; i++)
        {
            db.MenuPlanItems.Add(new MenuPlanItem
            {
                MenuPlanId = plan.Id,
                DayOfWeek = i % 7,
                MealType = (MealType)(i % 3),
                RecipeId = recipeIdsPerSlot[i]
            });
        }
        await db.SaveChangesAsync();

        return plan;
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_AggregatesSameIngredientAndUnitAcrossRecipes()
    {
        await using var db = CreateDb();
        var recipes = new[]
        {
            BuildRecipe(1, "Món A", (10, 0.3m, "kg")),
            BuildRecipe(2, "Món B", (10, 0.2m, "kg"))
        };
        var plan = await SeedPlanAsync(db, UserId, recipes, 1, 2);
        var service = new ShoppingListService(db);

        var list = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        var item = Assert.Single(list.Items);
        Assert.Equal(10, item.IngredientId);
        Assert.Equal("kg", item.Unit);
        Assert.Equal(0.5m, item.Quantity);
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_KeepsSeparateRowsForDifferentUnits()
    {
        await using var db = CreateDb();
        var recipes = new[]
        {
            BuildRecipe(1, "Món A", (10, 0.3m, "kg")),
            BuildRecipe(2, "Món B", (10, 200m, "g"))
        };
        var plan = await SeedPlanAsync(db, UserId, recipes, 1, 2);
        var service = new ShoppingListService(db);

        var list = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        Assert.Equal(2, list.Items.Count);
        Assert.Contains(list.Items, i => i.Unit == "kg" && i.Quantity == 0.3m);
        Assert.Contains(list.Items, i => i.Unit == "g" && i.Quantity == 200m);
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_RepeatedRecipeOccurrences_MultipliesQuantity()
    {
        await using var db = CreateDb();
        var recipes = new[] { BuildRecipe(1, "Món A", (10, 0.3m, "kg")) };
        var plan = await SeedPlanAsync(db, UserId, recipes, 1, 1, 1);
        var service = new ShoppingListService(db);

        var list = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        var item = Assert.Single(list.Items);
        Assert.Equal(0.9m, item.Quantity);
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_Regenerate_ReplacesOldItems()
    {
        await using var db = CreateDb();
        var recipes = new[]
        {
            BuildRecipe(1, "Món A", (10, 0.3m, "kg")),
            BuildRecipe(2, "Món B", (20, 0.1m, "kg"))
        };
        var plan = await SeedPlanAsync(db, UserId, recipes, 1);
        var service = new ShoppingListService(db);

        await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        db.MenuPlanItems.Add(new MenuPlanItem { MenuPlanId = plan.Id, DayOfWeek = 1, MealType = MealType.Lunch, RecipeId = 2 });
        await db.SaveChangesAsync();

        var second = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        Assert.Equal(1, await db.ShoppingLists.CountAsync());
        Assert.Equal(2, second.Items.Count);
        Assert.Contains(second.Items, i => i.IngredientId == 10 && i.Quantity == 0.3m);
        Assert.Contains(second.Items, i => i.IngredientId == 20 && i.Quantity == 0.1m);
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_NonexistentPlan_Throws()
    {
        await using var db = CreateDb();
        var service = new ShoppingListService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateFromMenuPlanAsync(UserId, 999));
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_PlanOwnedByDifferentUser_Throws()
    {
        await using var db = CreateDb();
        var recipes = new[] { BuildRecipe(1, "Món A", (10, 0.3m, "kg")) };
        var plan = await SeedPlanAsync(db, OtherUserId, recipes, 1);
        var service = new ShoppingListService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateFromMenuPlanAsync(UserId, plan.Id));
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_EmptyPlan_CreatesShoppingListWithNoItems()
    {
        await using var db = CreateDb();
        var plan = await SeedPlanAsync(db, UserId, []);
        var service = new ShoppingListService(db);

        var list = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        Assert.Empty(list.Items);
    }

    [Fact]
    public async Task GenerateFromMenuPlanAsync_PersistsMenuPlanIdAndUserId()
    {
        await using var db = CreateDb();
        var recipes = new[] { BuildRecipe(1, "Món A", (10, 0.3m, "kg")) };
        var plan = await SeedPlanAsync(db, UserId, recipes, 1);
        var service = new ShoppingListService(db);

        var list = await service.GenerateFromMenuPlanAsync(UserId, plan.Id);

        var saved = await db.ShoppingLists.FindAsync(list.Id);
        Assert.NotNull(saved);
        Assert.Equal(plan.Id, saved!.MenuPlanId);
        Assert.Equal(UserId, saved.UserId);
    }
}
