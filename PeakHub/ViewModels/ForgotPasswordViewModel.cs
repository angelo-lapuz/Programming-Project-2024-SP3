using System.ComponentModel.DataAnnotations;

namespace PeakHub.ViewModels{
    public class ForgotPasswordViewModel  {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
