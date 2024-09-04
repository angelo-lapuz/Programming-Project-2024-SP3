namespace WebAPI.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AwardID { get; set; }
        public virtual Award Award { get; set; }
        public string ProfileIMG { get; set; }
        public int TaskID { get; set; }
        public virtual Task Task { get; set; }

        // TODO - can be a list of Like datatypes
        public int Likes { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
