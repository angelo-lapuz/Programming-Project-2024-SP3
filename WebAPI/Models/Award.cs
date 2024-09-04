namespace WebAPI.Models
{
    public class Award
    {
        public int AwardID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }

        // TODO - double check condition datatype
        public string Condition { get; set; }
    }
}
