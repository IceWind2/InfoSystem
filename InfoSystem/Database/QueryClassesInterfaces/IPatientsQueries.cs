using System.Threading.Tasks;
using System.Collections.Generic;

namespace InfoSystem
{
    internal interface IPatientsQueries : IQueryClassBase
    {
        public Task<List<Patient>> GetPatientsListAsync();
        public Task<List<ArchivedPatient>> GetArchivedPatientsListAsync();
        public Task ArchiveOldPatientsAsync();

        public Patient AddPatient(CreateUpdatePatientDTO newPatient);
        public Task<Patient> UpdatePatientAsync(CreateUpdatePatientDTO patient);

        public Task<List<History>> GetPatientHistoryAsync(int patientId);
        public void WritePatientHistory(int patientId, string note, string changeNote);

        public void AddPatientMedicine(ChangePatientMedicineDTO patientMedicineDTO);
        public void RemovePatientMedicine(ChangePatientMedicineDTO patientMedicineDTO);
    }
}
