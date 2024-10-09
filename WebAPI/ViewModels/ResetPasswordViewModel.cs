using System.ComponentModel.DataAnnotations;

namespace WebAPI.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string UserName { get; set; }
    }


}
