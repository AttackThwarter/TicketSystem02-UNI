using System.ComponentModel.DataAnnotations;

namespace TicketSystem.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    [Display(Name = "ایمیل")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
    [DataType(DataType.Password)]
    [Display(Name = "تکرار رمز عبور")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
