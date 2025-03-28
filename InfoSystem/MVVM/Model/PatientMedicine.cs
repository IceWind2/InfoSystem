using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSystem
{
    [Table("patients_medicine")]
    [PrimaryKey(nameof(PatientId), nameof(MedicineId))]
    public class PatientMedicine
    {
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("Medicine")]
        public int MedicineId { get; set; }
        public virtual Medicine? Medicine{ get; set; }

        public string Prescription { get; set; }
    }
}
