namespace CookingAdvisor.Models;

// Bitmask of which meals a Recipe suits (a recipe can fit more than one meal).
// Kept separate from MealType (used as a single value on MenuPlanItem) because
// Breakfast = 0 there would break flag combination.
[Flags]
public enum MealTypeFlags
{
    None = 0,
    Breakfast = 1,
    Lunch = 2,
    Dinner = 4
}
