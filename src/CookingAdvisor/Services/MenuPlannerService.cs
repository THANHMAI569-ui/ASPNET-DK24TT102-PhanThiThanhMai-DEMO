using CookingAdvisor.Data;
using CookingAdvisor.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Services;

// Weekly menu planner (PLAN.md §5.2): greedy generator that fills 7 days × 3 meals,
// avoiding repeats while unused candidates remain, relaxing meal-type suitability
// (never the region filter) when the pool runs dry, and preferring favorites, then
// calorie balance against the daily target.
//
// When the recipe library is too small to fill 21 slots without repeating, repeats
// are spread evenly (least-used dish first) instead of following the preference
// order, so a thin library still yields a varied week.
public class MenuPlannerService(AppDbContext db)
{
    private const int DailyCalorieTarget = 2000;

    private static readonly (MealType Meal, MealTypeFlags Flag)[] Meals =
    [
        (MealType.Breakfast, MealTypeFlags.Breakfast),
        (MealType.Lunch, MealTypeFlags.Lunch),
        (MealType.Dinner, MealTypeFlags.Dinner)
    ];

    public async Task<MenuPlan> GenerateWeeklyPlanAsync(
        string userId, string planName, DateOnly weekStartDate, Region? region = null)
    {
        var recipesQuery = db.Recipes.AsQueryable();
        if (region is not null)
            recipesQuery = recipesQuery.Where(r => r.Region == region);

        var candidates = await recipesQuery
            .Select(r => new { r.Id, r.CaloriesPerServing, r.SuitableMealTypes })
            .ToListAsync();

        if (candidates.Count == 0)
            throw new InvalidOperationException("No recipes available to generate a menu plan.");

        var favoriteIds = (await db.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.RecipeId)
                .ToListAsync())
            .ToHashSet();

        var used = new HashSet<int>();
        var useCounts = new Dictionary<int, int>();
        var items = new List<MenuPlanItem>();

        for (var day = 0; day < 7; day++)
        {
            var dayCalories = 0;

            for (var mealIndex = 0; mealIndex < Meals.Length; mealIndex++)
            {
                var (meal, flag) = Meals[mealIndex];
                var mealTarget = DailyCalorieTarget * (mealIndex + 1) / Meals.Length;

                var suitable = candidates.Where(c => c.SuitableMealTypes.HasFlag(flag)).ToList();
                var pool = suitable.Where(c => !used.Contains(c.Id)).ToList();

                // Repeats only become necessary once every suitable dish has been
                // used; meal-type suitability is relaxed only after that.
                var repeating = pool.Count == 0;
                if (repeating) pool = suitable;
                if (pool.Count == 0) pool = candidates;

                var ordered = repeating
                    // Variety beats preference once repeats are unavoidable: without
                    // this key the calorie tie-break returns the same dish for every
                    // remaining slot, so one dish fills the week and the rest go unused.
                    ? pool.OrderBy(c => useCounts.GetValueOrDefault(c.Id))
                        .ThenByDescending(c => favoriteIds.Contains(c.Id))
                    : pool.OrderByDescending(c => favoriteIds.Contains(c.Id));

                var chosen = ordered
                    .ThenBy(c => Math.Abs(dayCalories + c.CaloriesPerServing - mealTarget))
                    .ThenBy(c => c.Id)
                    .First();

                dayCalories += chosen.CaloriesPerServing;
                used.Add(chosen.Id);
                useCounts[chosen.Id] = useCounts.GetValueOrDefault(chosen.Id) + 1;
                items.Add(new MenuPlanItem { DayOfWeek = day, MealType = meal, RecipeId = chosen.Id });
            }
        }

        var plan = new MenuPlan
        {
            UserId = userId,
            Name = planName,
            WeekStartDate = weekStartDate,
            CreatedAt = DateTime.UtcNow,
            Items = items
        };

        db.MenuPlans.Add(plan);
        await db.SaveChangesAsync();
        return plan;
    }
}
