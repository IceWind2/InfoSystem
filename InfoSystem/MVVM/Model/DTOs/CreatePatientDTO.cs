using System;
using System.Collections.Generic;

namespace InfoSystem
{
    internal class CreatePatientDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate{ get; set; }

        public string Gender { get; set; }

        public int LocationId { get; set; }

        public int DiagnosisId { get; set; }

        public HashSet<int> MedicineIds { get; set; }
    }
}
