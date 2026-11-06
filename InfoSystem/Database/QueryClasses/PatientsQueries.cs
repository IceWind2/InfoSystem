using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSystem
{
    internal sealed class PatientsQueries : QueryClassBase, IPatientsQueries
    {
        public Task<List<Patient>> GetPatientsListAsync()
        {
            return _dbContext.Patients
                .AsNoTracking()
                .Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                .Include(p => p.Location)
                .Include(p => p.Diagnosis)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public Task<List<ArchivedPatient>> GetArchivedPatientsListAsync()
        {
            return _dbContext.ArchivedPatients
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task ArchiveOldPatientsAsync()
        {
            var dateNow = DateTime.UtcNow;
            var patientsToArchive = await _dbContext.Patients
                .AsNoTracking()
                .Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                .Include(p => p.Location)
                .Include(p => p.Diagnosis)
                .Where(p => dateNow.Year - p.BirthDate.Year >= 18 &&
                            dateNow.Month >= p.BirthDate.Month &&
                            dateNow.Day >= p.BirthDate.Day)
                .ToListAsync();

            foreach (Patient patient in patientsToArchive)
            {
                var newArchiveEntry = new ArchivedPatient
                {
                    PatientId = patient.Id,
                    Name = patient.Name,
                    MedicineView = patient.MedicineView,
                    BirthDate = patient.BirthDate,
                    DisplaySex = patient.DisplaySex,
                    LocationView = patient.Location?.Name,
                    DiagnosisView = patient.Diagnosis?.Name
                };

                _dbContext.ArchivedPatients.Add(newArchiveEntry);
                _dbContext.Patients.Remove(patient);
            }
        }

        public Patient AddPatient(CreateUpdatePatientDTO newPatient)
        {
            var patient = new Patient
            {
                Name = newPatient.Name.Trim(),
                BirthDate = newPatient.BirthDate,
                Sex = newPatient.Sex,
                LocationId = newPatient.LocationId
            };
            _dbContext.Patients.Add(patient);

            return patient;
        }

        public async Task<Patient> UpdatePatientAsync(CreateUpdatePatientDTO updatePatientDto)
        {
            var patient = await _dbContext.Patients
                .FirstAsync(p => p.Id == updatePatientDto.Id);

            patient.Name = updatePatientDto.Name;
            patient.BirthDate = updatePatientDto.BirthDate;
            patient.Sex = updatePatientDto.Sex;
            patient.Location = await _dbContext.Locations.FirstAsync(l => l.Id == updatePatientDto.LocationId);

            return patient;
        }

        public Task<List<History>> GetPatientHistoryAsync(int patientId)
        {
            return _dbContext.History
                .AsNoTracking()
                .Where(h => h.PatientId == patientId)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        public void AddPatientMedicine(ChangePatientMedicineDTO patientMedicineDTO)
        {
            var patientMedicine = new PatientMedicine()
            {
                PatientId = patientMedicineDTO.PatientId,
                MedicineId = patientMedicineDTO.MedicineId,
                Prescription = patientMedicineDTO.Prescription
            };

            _dbContext.PatientsMedicine.Add(patientMedicine);
        }

        public void RemovePatientMedicine(ChangePatientMedicineDTO patientMedicineDTO)
        {
            var patientMedicine = new PatientMedicine
            {
                PatientId = patientMedicineDTO.PatientId,
                MedicineId = patientMedicineDTO.MedicineId
            };
            _dbContext.PatientsMedicine.Remove(patientMedicine);
        }

        public void WritePatientHistory(int patientId, string note, string changeNote)
        {
            var newPatientHistory = new History()
            {
                PatientId = patientId,
                Timestamp = DateTime.UtcNow,
                Note = note,
                Change = changeNote
            };

            _dbContext.History.Add(newPatientHistory);
        }
    }
}
