using System.ComponentModel.DataAnnotations;

namespace CookingAdvisor.ViewModels;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    // [Required] lets a whitespace-only value through; after the controller's
    // Trim() that becomes an empty name, and the home page monogram
    // (Name.Substring(0, 1)) would then crash on it.
    [RegularExpression(@".*\S.*", ErrorMessage = "Vui lòng nhập tên danh mục.")]
    [StringLength(100)]
    [Display(Name = "Tên danh mục")]
    public string Name { get; set; } = string.Empty;
}
