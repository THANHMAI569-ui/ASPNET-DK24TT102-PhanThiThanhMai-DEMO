using System.ComponentModel.DataAnnotations;
using CookingAdvisor.Models;

namespace CookingAdvisor.ViewModels;

public class MenuGenerateViewModel
{
    [Required(ErrorMessage = "Vui lòng đặt tên thực đơn.")]
    public string Name { get; set; } = "Thực đơn tuần";

    [Required]
    [DataType(DataType.Date)]
    public DateOnly WeekStartDate { get; set; }

    public Region? Region { get; set; }
}
