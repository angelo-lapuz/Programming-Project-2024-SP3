using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Models
{
    public class User : IdentityUser
    {

        public virtual List<Award> Awards { get; set; }
        public string ProfileIMG { get; set; }
        // public int TaskID { get; set; }
        // public virtual Task Task { get; set; }

        public virtual List<Peak> Peaks { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}
