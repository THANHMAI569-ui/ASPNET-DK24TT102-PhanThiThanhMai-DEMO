namespace CookingAdvisor.Models;

// Sort options offered on the recipe list. Binding the query string to an enum
// makes the enum itself the whitelist: anything the user types that is not one
// of these values binds to the default and is ignored, so no caller-supplied
// text ever reaches the ordering expression.
public enum RecipeSortOrder
{
    Name,
    TimeAsc,
    CaloriesAsc,
    CaloriesDesc
}
