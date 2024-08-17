using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [InverseProperty("Patient")]
        public virtual ICollection<PatientMedicine> PatientMedicine { get; set; }
    }
}
