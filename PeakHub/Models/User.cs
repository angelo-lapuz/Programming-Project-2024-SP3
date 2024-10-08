﻿using System.ComponentModel.DataAnnotations.Schema;
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

    /// <summary>
    ///  .net relationship table for many to many relationship between User and Award
    /// </summary>
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

