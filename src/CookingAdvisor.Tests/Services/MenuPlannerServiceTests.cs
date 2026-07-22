using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.Services;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Tests.Services;

public class MenuPlannerServiceTests
{
    private const string UserId = "user-1";
    private static readonly DateOnly WeekStart = new(2026, 7, 27);

    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task SeedAsync(AppDbContext db, params Recipe[] recipes)
    {
        var category = new Category { Id = 1, Name = "Món chính" };
        db.Categories.Add(category);

        foreach (var recipe in recipes)
        {
            recipe.CategoryId = category.Id;
            db.Recipes.Add(recipe);
        }

        await db.SaveChangesAsync();
    }

    private static Recipe BuildRecipe(
        int id, string name, int calories, MealTypeFlags mealTypes, Region region = Region.North) =>
        new()
        {
            Id = id,
            Name = name,
            Servings = 4,
            CaloriesPerServing = calories,
            SuitableMealTypes = mealTypes,
            Region = region
        };

    private const MealTypeFlags AllMeals =
        MealTypeFlags.Breakfast | MealTypeFlags.Lunch | MealTypeFlags.Dinner;

    [Fact]
    public async Task GenerateWeeklyPlanAsync_FillsAll21Slots()
    {
        await using var db = CreateDb();
        await SeedAsync(db, Enumerable.Range(1, 10)
            .Select(i => BuildRecipe(i, $"Món {i}", 300 + i * 10, AllMeals))
            .ToArray());
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart);

        Assert.Equal(21, plan.Items.Count);
        foreach (var day in Enumerable.Range(0, 7))
        {
            var mealsForDay = plan.Items.Where(i => i.DayOfWeek == day).Select(i => i.MealType).ToList();
            Assert.Equal([MealType.Breakfast, MealType.Lunch, MealType.Dinner], mealsForDay);
        }
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_EnoughRecipes_NoRepeatsWithinWeek()
    {
        await using var db = CreateDb();
        await SeedAsync(db, Enumerable.Range(1, 25)
            .Select(i => BuildRecipe(i, $"Món {i}", 300 + i * 5, AllMeals))
            .ToArray());
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart);

        Assert.Equal(21, plan.Items.Select(i => i.RecipeId).Distinct().Count());
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_FewRecipes_AllowsRepeatsButStillFills21()
    {
        await using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Món A", 400, AllMeals),
            BuildRecipe(2, "Món B", 450, AllMeals));
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart);

        Assert.Equal(21, plan.Items.Count);
        Assert.True(plan.Items.Select(i => i.RecipeId).Distinct().Count() < 21);
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_RespectsRegionFilter()
    {
        await using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Món Bắc 1", 350, AllMeals, Region.North),
            BuildRecipe(2, "Món Bắc 2", 380, AllMeals, Region.North),
            BuildRecipe(3, "Món Nam 1", 400, AllMeals, Region.South),
            BuildRecipe(4, "Món Nam 2", 420, AllMeals, Region.South),
            BuildRecipe(5, "Món Nam 3", 360, AllMeals, Region.South));
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart, Region.South);

        Assert.Equal(21, plan.Items.Count);
        Assert.All(plan.Items, item => Assert.True(item.RecipeId is 3 or 4 or 5));
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_PrefersFavoriteRecipe()
    {
        await using var db = CreateDb();
        await SeedAsync(db,
            BuildRecipe(1, "Không yêu thích", 400, AllMeals),
            BuildRecipe(2, "Món yêu thích", 400, AllMeals));
        db.Favorites.Add(new Favorite { UserId = UserId, RecipeId = 2 });
        await db.SaveChangesAsync();
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart);

        var firstSlot = plan.Items.Single(i => i.DayOfWeek == 0 && i.MealType == MealType.Breakfast);
        Assert.Equal(2, firstSlot.RecipeId);
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_NoRecipesAvailable_Throws()
    {
        await using var db = CreateDb();
        var service = new MenuPlannerService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart));
    }

    [Fact]
    public async Task GenerateWeeklyPlanAsync_PersistsMenuPlanAndItems()
    {
        await using var db = CreateDb();
        await SeedAsync(db, Enumerable.Range(1, 10)
            .Select(i => BuildRecipe(i, $"Món {i}", 300 + i * 10, AllMeals))
            .ToArray());
        var service = new MenuPlannerService(db);

        var plan = await service.GenerateWeeklyPlanAsync(UserId, "Thực đơn tuần", WeekStart);

        Assert.Equal(1, await db.MenuPlans.CountAsync());
        Assert.Equal(21, await db.MenuPlanItems.CountAsync(i => i.MenuPlanId == plan.Id));
        var saved = await db.MenuPlans.FindAsync(plan.Id);
        Assert.NotNull(saved);
        Assert.Equal(UserId, saved!.UserId);
        Assert.Equal(WeekStart, saved.WeekStartDate);
    }
}
