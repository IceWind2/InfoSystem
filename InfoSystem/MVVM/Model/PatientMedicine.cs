using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSystem
{
    [Table("patients_medicine")]
    internal class PatientMedicine
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("Medicine")]
        public int MedicineId { get; set; }
        public virtual Medicine? Medicine { get; set; }
    }
}
