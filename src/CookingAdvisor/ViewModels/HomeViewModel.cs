namespace CookingAdvisor.ViewModels;

public class HomeViewModel
{
    public int RecipeCount { get; set; }
    public int IngredientCount { get; set; }

    public IReadOnlyList<RecipeListItemViewModel> FeaturedRecipes { get; set; } = [];
    public IReadOnlyList<HomeCategoryViewModel> Categories { get; set; } = [];

    /// <summary>Photos used as the hero collage, newest seeded dishes first.</summary>
    public IReadOnlyList<string> HeroImageUrls { get; set; } = [];
}

public class HomeCategoryViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RecipeCount { get; set; }
}
