using System;

namespace InfoSystem
{
    internal sealed class CreatePatientDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate{ get; set; }

        public Sex Sex { get; set; }

        public int LocationId { get; set; }

        public string Address { get; set; }
    }
}
