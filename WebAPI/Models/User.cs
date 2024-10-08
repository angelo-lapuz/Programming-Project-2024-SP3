using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Awards = new List<Award>();
            Peaks = new List<Peak>();
            Posts = new List<Post>();
            Likes = new List<Like>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public virtual List<Award> Awards { get; set; }
        public string ProfileIMG { get; set; }
        public virtual List<Peak> Peaks { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }

    /// <summary>
    ///  .net relationship table for many to many relationship between User and Award
    /// </summary>
    public class AwardUser
    {
        public string UserID { get; set; }
        public virtual User User { get; set; }

        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
    }

    /// <summary>
    /// .net relationship table for many to many relationship between User and Peak
    /// </summary>
    public class PeakUser
    {
        public string UserID { get; set; }
        public virtual User User { get; set; }

        public int PeakID { get; set; }
        public virtual Peak Peak { get; set; }
    }


}
