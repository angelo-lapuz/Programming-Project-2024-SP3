namespace WebAPI.Models
{
    public class Award
    {
        public int AwardID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }

        // TODO - double check condition 
        public string Condition { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
