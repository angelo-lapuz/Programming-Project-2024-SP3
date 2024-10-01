using System.ComponentModel.DataAnnotations;

namespace PeakHub.ViewModels{
    public class LoginViewModel  {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // 

        [Required]
        [Display(Name = "Username")]
        public string UserName_forgot { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password_forgot { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password_forgot", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
