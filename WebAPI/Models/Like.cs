using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Like
    {
        // fix me 
        public int LikeID { get; set; }

        [Required]
        [ForeignKey("Post")]
        public int PostID { get; set; }
        [JsonIgnore]
        public virtual Post Post { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
