namespace WebAPI.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string Name { get; set; }
        public string IMG { get; set; }
        public string Details { get; set; }
        public string Coords { get; set; }
        public char AccountType { get; set; }
    }
}
