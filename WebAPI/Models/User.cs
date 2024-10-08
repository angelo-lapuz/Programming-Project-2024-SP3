using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ProfileIMG { get; set; }
        public string Routes { get; set; }

       
        public virtual ICollection<UserPeak> UserPeaks { get; set; }
        public virtual ICollection<UserAward> UserAwards { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }

    public class UserAward
    {
        public int Id { get; set; }

        public string UserID { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
    }

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
