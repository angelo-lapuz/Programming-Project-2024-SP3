using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PeakHub.Models
{
    public class Peak
    {
        public int PeakID { get; set; }
        public string Name { get; set; }
        public string IMG { get; set; }
        public string Details { get; set; }
        public string Region { get; set; }
        public string Coords { get; set; }
        public int Elevation { get; set; }
        public string Routes { get; set; }
        public string Difficulty { get; set; }

    }
}
