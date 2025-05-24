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
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [FilterProperty]
        public string Name { get; set; }

        [NotMapped]
        [FilterProperty]
        public string DisplaySex
        {
            get
            {
                return Sex switch
                {
                    Sex.Male => "М",
                    Sex.Female => "Ж",
                    _ => throw new ArgumentException("Invalid sex value.")
                };
            }
        }
        public Sex Sex { get; set; }

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
        public int? LocationId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [FilterProperty]
        public virtual Location? Location { get; set; }

        [ForeignKey("Diagnosis")]
        public int? DiagnosisId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [FilterProperty]
        public virtual Diagnosis? Diagnosis { get; set; }

        [NotMapped]
        public string MedicineView
        {
            get
            {
                var medicineList = string.Join(", ", PatientMedicine!.Select(pm => pm.Medicine!.Name).Order());
                if (string.IsNullOrEmpty(medicineList))
                {
                    medicineList = "---";
                }
                return medicineList;
            }
        }

        public virtual ICollection<PatientMedicine>? PatientMedicine { get; set; }

        [NotMapped]
        [FilterProperty]
        public string MedicineDetails
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var med in PatientMedicine!)
                {
                    sb.AppendLine($"{med.Medicine!.Name}  \u2014  {med.Prescription}");
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendJoin(" | ", Name, DisplaySex, Age, Diagnosis?.Name, MedicineView, Location?.Name);
            return sb.ToString();
        }
    }
}
