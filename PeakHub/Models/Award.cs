﻿using Newtonsoft.Json;

namespace PeakHub.Models
{
    public class Award
    {
        public int AwardID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Condition { get; set; }

        // prevents cyclic import error with lazy loading
        [JsonIgnore]
        public virtual ICollection<UserAward> UserAwards { get; set; }
    }
}
