using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InfoSystem
{
    [Table("history")]
    [PrimaryKey(nameof(PatientId), nameof(Timestamp))]
    internal class History
    {
        [Required]
        public string PatientData {  get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [NotMapped]
        public DateTime LocalTime
        {
            get
            {
                return Timestamp.ToLocalTime();
            }
        }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
