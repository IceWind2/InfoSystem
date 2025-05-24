using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSystem
{
    [Table("archived_patients")]
    public class ArchivedPatient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [FilterProperty]
        public string Name { get; set; }

        [Required]
        [FilterProperty]
        public string DisplaySex { get; set; }

        [Required]
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

        [FilterProperty]
        public string? LocationView { get; set; }

        [FilterProperty]
        public string? DiagnosisView { get; set; }

        [FilterProperty]
        public string? MedicineView { get; set; }
    }
}
