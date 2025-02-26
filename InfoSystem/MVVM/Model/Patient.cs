using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace InfoSystem
{
    [Table("patients")]
    internal class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [FilterProperty]
        public string Name { get; set; }
        [FilterProperty]
        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }
        
        [NotMapped]
        [FilterProperty]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Date.Year;
                if (BirthDate.AddYears(age) > today) age--;
                return age;
            }
        }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [FilterProperty]
        public Location? Location { get; set; }

        [ForeignKey("Diagnosis")]
        public int DiagnosisId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [FilterProperty]
        public Diagnosis? Diagnosis { get; set; }

        [NotMapped]
        [FilterProperty]
        public string MedicineView
        {
            get
            {
                var medicineList = string.Join(", ", Medicine!.Select(m => m.Name).Order());
                if (string.IsNullOrEmpty(medicineList))
                {
                    medicineList = "---";
                }
                return medicineList;
            }
        }

        public virtual ICollection<Medicine>? Medicine { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendJoin(" | ", Name, Gender, Age, Diagnosis!.Name, MedicineView, Location!.Name);
            return sb.ToString();
        }
    }
}
