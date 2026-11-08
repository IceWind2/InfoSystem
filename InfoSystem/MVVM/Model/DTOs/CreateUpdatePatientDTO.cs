using System;

namespace InfoSystem
{
    internal sealed class CreateUpdatePatientDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate{ get; set; }

        public Sex Sex { get; set; }

        public Location Location { get; set; }

        public Diagnosis? Diagnosis { get; set; }

        public string? Note { get; set; }
    }
}
