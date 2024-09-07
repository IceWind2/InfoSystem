using System.Collections.Generic;

namespace InfoSystem
{
    internal class CreatePatientDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }


        public HashSet<int> MedicineIds { get; set; }
    }
}
