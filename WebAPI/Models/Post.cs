﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public string Content { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "datetime2")]
        public DateTime TransactionTimeUtc { get; set; }
        
        // TODO - Can be a list of Likes
        public int Likes { get; set; }
        public string MediaType { get; set; }
        public string MediaLink { get; set; }
        public int BoardID { get; set; }
        public virtual Board Board { get; set; }

    }
}