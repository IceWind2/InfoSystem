namespace InfoSystem
{
    public class AddPatientMedicineDTO
    {
        public int PatientId { get; set; }
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public string Prescription { get; set; }
        public string? Note { get; set; }
    }
}
