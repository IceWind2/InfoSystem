using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace InfoSystem
{
    [Table("patients")]
    internal class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        [NotMapped]
        public string MedicineView
        {
            get
            {
                return string.Join(", ", Medicine!.Select(m => m.Name).Order());
            }
        }

        public virtual ICollection<Medicine>? Medicine { get; set; }
    }
}
