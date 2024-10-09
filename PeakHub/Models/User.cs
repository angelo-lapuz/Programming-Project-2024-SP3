using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PeakHub.Models
{
    public class User : IdentityUser
    {

        public virtual List<Award> Awards { get; set; }
        public string ProfileIMG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public string Routes { get; set; }
        public virtual List<Peak> Peaks { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}
