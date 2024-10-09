using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

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

        public virtual List<Award> Awards { get; set; }
        public string ProfileIMG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Routes { get; set; }
        [JsonIgnore] public virtual List<Peak> Peaks { get; set; }
        [JsonIgnore] public virtual List<Post> Posts { get; set; }
        [JsonIgnore] public virtual List<Like> Likes { get; set; }
    }

    /// <summary>
    ///  .net relationship table for many to many relationship between User and Award
    /// </summary>
    public class AwardUser
    {
        public int AwardUserID { get; set; }
        public string UserID { get; set; }
        [JsonIgnore] public virtual User User { get; set; }

        public int AwardID { get; set; }
        [JsonIgnore] public virtual Award Award { get; set; }
    }

    /// <summary>
    /// .net relationship table for many to many relationship between User and Peak
    /// </summary>
    public class PeakUser
    {
        public int userPeakID { get; set; }
        public string UserID { get; set; }

        [JsonIgnore] public virtual User User { get; set; }

        public int PeakID { get; set; }
        [JsonIgnore] public virtual Peak Peak { get; set; }
    }


}
