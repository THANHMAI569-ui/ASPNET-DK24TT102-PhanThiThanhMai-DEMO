namespace CookingAdvisor.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
