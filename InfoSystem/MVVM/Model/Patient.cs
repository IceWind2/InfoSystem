using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

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
        public required string Name { get; set; }

        public Sex Sex { get; set; }

        public DateTime BirthDate { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        [FilterProperty]
        public virtual Location? Location { get; set; }

        [ForeignKey("Diagnosis")]
        public int? DiagnosisId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        [FilterProperty]
        public virtual Diagnosis? Diagnosis { get; set; }

        public virtual ObservableCollection<PatientMedicine> PatientMedicine { get; set; } = new();

        // Properties for views
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

        [NotMapped]
        [FilterProperty]
        public Medicine[] TopMedicine
        {
            get
            {
                if (PatientMedicine == null)
                {
                    return Enumerable.Empty<Medicine>().ToArray();
                }

                return PatientMedicine.Select(pm => pm.Medicine!).Take(2).ToArray();
            }
        }

        [NotMapped]
        public int MoreMedicineCount => PatientMedicine == null || PatientMedicine.Count <= 2 ? 0 : PatientMedicine.Count - 2;
    }
}
