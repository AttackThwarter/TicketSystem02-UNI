using System.ComponentModel.DataAnnotations;

namespace TicketSystem.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    [Display(Name = "ایمیل")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;
}
