namespace PeakHub.Models
{
    public class Board
    {
        public int BoardID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
