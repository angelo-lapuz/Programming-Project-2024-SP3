using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PeakHub.Models
{
    public class Like
    {
        public int LikeID { get; set; }
        [Required]
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
        [Required]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
    }
}
