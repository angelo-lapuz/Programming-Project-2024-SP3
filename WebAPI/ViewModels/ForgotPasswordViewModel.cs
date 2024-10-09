using System.ComponentModel.DataAnnotations;

namespace WebAPI.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}
