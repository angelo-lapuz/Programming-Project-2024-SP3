using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PeakHub.Models
{
    public class Like
    {
        public int LikeID { get; set; }

        [Required]
        [ForeignKey("Post")]
        public int PostID { get; set; }
        public virtual Post Post { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }
}
