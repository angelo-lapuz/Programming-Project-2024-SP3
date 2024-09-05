using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class User
    {
        [Display(Name = "User ID")]
        public int UserID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(94)]
        public string Password { get; set; }

        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
        public string ProfileIMG { get; set; }
        public int TaskID { get; set; }
        public virtual Task Task { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}
