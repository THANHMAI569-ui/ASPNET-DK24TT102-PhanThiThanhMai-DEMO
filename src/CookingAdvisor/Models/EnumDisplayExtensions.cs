namespace CookingAdvisor.Models;

public static class EnumDisplayExtensions
{
    public static string ToDisplayName(this Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => "Dễ",
        Difficulty.Medium => "Trung bình",
        Difficulty.Hard => "Khó",
        _ => difficulty.ToString()
    };

    public static string ToDisplayName(this Region region) => region switch
    {
        Region.North => "Miền Bắc",
        Region.Central => "Miền Trung",
        Region.South => "Miền Nam",
        _ => region.ToString()
    };

    public static string ToDisplayName(this RecipeSortOrder sort) => sort switch
    {
        RecipeSortOrder.Name => "Tên A-Z",
        RecipeSortOrder.TimeAsc => "Nấu nhanh",
        RecipeSortOrder.CaloriesAsc => "Calo thấp",
        RecipeSortOrder.CaloriesDesc => "Calo cao",
        _ => sort.ToString()
    };
}
