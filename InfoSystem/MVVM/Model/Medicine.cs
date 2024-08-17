using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSystem
{
    [Table("medicine")]
    internal class Medicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty("Medicine")]
        public virtual ICollection<PatientMedicine> PatientMedicine{ get; set; }
    }
}
