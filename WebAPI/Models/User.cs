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
            UserAwards = new List<UserAward>();
            UserPeaks = new List<UserPeak>();
            Posts = new List<Post>();
            Likes = new List<Like>();
        }

        
        public string ProfileIMG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Routes { get; set; }
        public virtual List<UserPeak> UserPeaks { get; set; }
        public virtual List<UserAward> UserAwards { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }

    /// <summary>
    ///  .net relationship table for many to many relationship between User and Award
    /// </summary>
    public class UserAward
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
    }

    /// <summary>
    /// .net relationship table for many to many relationship between User and Peak
    /// </summary>
    public class UserPeak
    {
        public int Id { get; set; }
        public string UserID { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public int PeakID { get; set; }
        public virtual Peak Peak { get; set; }
    }


}
