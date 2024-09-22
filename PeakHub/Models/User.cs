using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PeakHub.Models
{
    public class User
    {
        [Display(Name = "User ID")]
        public int UserID { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(94)]
        public string Password { get; set; }

        public virtual List<Award> Awards { get; set; }
        public string ProfileIMG { get; set; }
        //public int TaskID { get; set; }
        //public virtual Task Task { get; set; }

        public virtual List<Peak> Tasks { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}
