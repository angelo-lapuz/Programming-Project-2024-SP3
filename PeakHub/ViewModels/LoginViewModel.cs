using System.ComponentModel.DataAnnotations;

namespace PeakHub.ViewModels{
    public class LoginViewModel  {

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
