using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Peak
    {
        public int PeakID { get; set; }
        public string Name { get; set; }
        public string IMG { get; set; }
        public string Details { get; set; }
        public string Coords { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(1)]
        [Display(Name = "Account Type")]
        public char Difficulty { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public string Routes { get; set; }
    }
}
