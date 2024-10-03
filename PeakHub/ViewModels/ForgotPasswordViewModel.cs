using System.ComponentModel.DataAnnotations;

namespace PeakHub.ViewModels{
    public class ForgotPasswordViewModel  {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}
