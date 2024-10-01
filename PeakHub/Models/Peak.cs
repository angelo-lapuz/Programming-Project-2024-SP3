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

        public string Section { get; set; }
        public string Coords { get; set; }

        public int Elevation { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(1)]
        [Display(Name = "Difficulty")]
        public char Difficulty { get; set; }
    }
}
