using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Like
    {
        [Required]
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
        [Required]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
    }
}
