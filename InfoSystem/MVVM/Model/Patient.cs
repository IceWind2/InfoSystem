using Microsoft.EntityFrameworkCore;
using System;
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
        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }
        
        [NotMapped]
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
        public Location? Location { get; set; }

        [ForeignKey("Diagnosis")]
        public int DiagnosisId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Diagnosis? Diagnosis { get; set; }

        [NotMapped]
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
    }
}
