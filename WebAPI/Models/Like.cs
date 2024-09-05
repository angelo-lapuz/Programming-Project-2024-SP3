namespace WebAPI.Models
{
    public class Like
    {
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
    }
}
