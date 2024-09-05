namespace WebAPI.Models
{
    public class Board
    {
        public int BoardID { get; set; }
        public string Name { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
