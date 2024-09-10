using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string Name { get; set; }
        public string IMG { get; set; }
        public string Details { get; set; }
        public string Coords { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(1)]
        [Display(Name = "Account Type")]
        public char AccountType { get; set; }
    }
}
