using System.ComponentModel.DataAnnotations;

namespace CookingAdvisor.ViewModels;

public class IngredientFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên nguyên liệu.")]
    [StringLength(100)]
    [Display(Name = "Tên nguyên liệu")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập đơn vị.")]
    [StringLength(20)]
    [Display(Name = "Đơn vị")]
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nhóm nguyên liệu.")]
    [StringLength(50)]
    [Display(Name = "Nhóm")]
    public string Group { get; set; } = string.Empty;
}
