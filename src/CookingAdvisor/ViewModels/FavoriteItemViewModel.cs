using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class FavoriteItemViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public Region Region { get; set; }
    public Difficulty Difficulty { get; set; }
}
