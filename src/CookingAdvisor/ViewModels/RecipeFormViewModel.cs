using System.ComponentModel.DataAnnotations;
using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class RecipeFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên món.")]
    [StringLength(150)]
    [Display(Name = "Tên món")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Cách làm")]
    public string? Instructions { get; set; }

    [Range(1, 100, ErrorMessage = "Khẩu phần phải từ 1 đến 100.")]
    [Display(Name = "Khẩu phần")]
    public int Servings { get; set; } = 4;

    [Range(0, 10000, ErrorMessage = "Thời gian không hợp lệ.")]
    [Display(Name = "Thời gian sơ chế (phút)")]
    public int PrepMinutes { get; set; }

    [Range(0, 10000, ErrorMessage = "Thời gian không hợp lệ.")]
    [Display(Name = "Thời gian nấu (phút)")]
    public int CookMinutes { get; set; }

    [Range(0, 100000, ErrorMessage = "Calo không hợp lệ.")]
    [Display(Name = "Calo / khẩu phần")]
    public int CaloriesPerServing { get; set; }

    [Display(Name = "Độ khó")]
    public Difficulty Difficulty { get; set; }

    [Display(Name = "Vùng miền")]
    public Region Region { get; set; }

    [StringLength(500)]
    [Display(Name = "Ảnh (URL)")]
    public string? ImageUrl { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    public List<RecipeIngredientInputViewModel> Ingredients { get; set; } = [];

    // Populated by the controller for the dropdowns; not bound from the form.
    public IReadOnlyList<CategoryOptionViewModel> CategoryOptions { get; set; } = [];
    public IReadOnlyList<IngredientOptionViewModel> IngredientOptions { get; set; } = [];
}

public class RecipeIngredientInputViewModel
{
    public int IngredientId { get; set; }

    [Range(0, 100000, ErrorMessage = "Số lượng không hợp lệ.")]
    public decimal Quantity { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }
}

public class IngredientOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
