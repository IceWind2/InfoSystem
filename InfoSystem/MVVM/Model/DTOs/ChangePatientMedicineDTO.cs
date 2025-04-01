namespace InfoSystem
{
    public class ChangePatientMedicineDTO
    {
        public int PatientId { get; set; }
        public int MedicineId { get; set; }
        public string Prescription { get; set; }
        public string? Note { get; set; }
    }
}
