namespace WebAPI.Models
{
    public class Board
    {
        public int BoardID { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public string ImageLink { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
