using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WebAPI.Models;

namespace PeakHub.Models
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

        public virtual List<UserAward> UserAwards { get; set; }
        public string ProfileIMG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public string Routes { get; set; }
        public bool IsBanned { get; set; }
        public virtual List<UserPeak> UserPeaks { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Like> Likes { get; set; }
    }


    public class UserAward
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Award")]
        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
    }

    public class UserPeak
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Peak")]
        public int PeakID { get; set; }
        public virtual Peak Peak { get; set; }
    }
}
