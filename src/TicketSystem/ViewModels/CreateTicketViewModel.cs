using System.ComponentModel.DataAnnotations;

namespace TicketSystem.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [Display(Name = "عنوان")]
        [StringLength(200, ErrorMessage = "عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "توضیحات الزامی است")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; } = string.Empty;
    }
}
