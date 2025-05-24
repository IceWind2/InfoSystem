using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InfoSystem
{
    internal static class DatabaseManager
    {
        #region Patients

        public static List<Patient> GetAllPatients()
        {
            using var context = new InfoContext();
            return context.Patients.AsNoTracking().Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                                                  .Include(p => p.Location)
                                                  .Include(p => p.Diagnosis)
                                                  .OrderBy(p => p.Id)
                                                  .ToList();
        }

        public static List<ArchivedPatient> GetAllArchivedPatients()
        {
            using var context = new InfoContext();
            return context.ArchivedPatients.AsNoTracking().OrderBy(p => p.Id).ToList();
        }

        public static Patient GetPatient(int patientId, InfoContext? infoContext = null)
        {
            using var context = infoContext ?? new InfoContext();
            return context.Patients.Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                                   .Include(p => p.Diagnosis)
                                   .Include(p => p.Location)
                                   .First(p => p.Id == patientId);
        }

        public static async Task<Patient> AddPatient(CreatePatientDTO createPatient)
        {
            using var context = new InfoContext();
            var patient = new Patient
            {
                Name = createPatient.Name.Trim(),
                BirthDate = createPatient.BirthDate,
                Sex = createPatient.Sex,
                LocationId = createPatient.LocationId
            };
            context.Patients.Add(patient);

            await context.SaveChangesAsync();

            // Add enty to the history
            await context.Entry(patient).Collection(p => p.PatientMedicine!).LoadAsync();
            var newPatientHistory = new History()
            {
                PatientId = patient.Id,
                PatientData = patient.ToString(),
                Timestamp = DateTime.UtcNow,
                Note = createPatient.Note,
                Change = HistoryChange.NewPatient
            };
            context.History.Add(newPatientHistory);

            await context.SaveChangesAsync();

            return patient;
        }

        public static async Task<Patient> UpdatePatient(CreatePatientDTO updatePatient)
        {
            using var context = new InfoContext();

            var patient = GetPatient(updatePatient.Id, context);
            patient.Name = updatePatient.Name;
            patient.BirthDate = updatePatient.BirthDate;
            patient.Sex = updatePatient.Sex;
            patient.Location = context.Locations.First(l => l.Id == updatePatient.LocationId);

            // Add enty to the history
            var newPatientHistory = new History()
            {
                PatientId = patient.Id,
                PatientData = patient.ToString(),
                Timestamp = DateTime.UtcNow,
                Note = updatePatient.Note,
                Change = HistoryChange.PatientUpdate
            };
            context.History.Add(newPatientHistory);

            await context.SaveChangesAsync();

            return patient;
        }

        public static Task RemovePatient(Patient patient)
        {
            using var context = new InfoContext();
            context.Patients.Remove(patient);
            return context.SaveChangesAsync();
        }

        public static Task<List<History>> GetPatientHistory(int patientId)
        {
            using var context = new InfoContext();
            return context.History.Where(h => h.PatientId == patientId)
                                  .OrderByDescending(h => h.Timestamp)
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        public static async Task AddPatientMedicine(ChangePatientMedicineDTO patientMedicineDTO)
        {
            using var context = new InfoContext();
            var patientMedicine = new PatientMedicine()
            {
                PatientId = patientMedicineDTO.PatientId,
                MedicineId = patientMedicineDTO.MedicineId,
                Prescription = patientMedicineDTO.Prescription
            };
            context.PatientsMedicine.Add(patientMedicine);
            await context.SaveChangesAsync();

            Patient patient = await context.Patients.Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                                                    .FirstAsync(p => p.Id == patientMedicineDTO.PatientId);
            var newPatientHistory = new History()
            {
                PatientId = patient.Id,
                PatientData = patient.ToString(),
                Timestamp = DateTime.UtcNow,
                Note = patientMedicineDTO.Note,
                Change = HistoryChange.MedicineAdded
            };
            context.History.Add(newPatientHistory);
            await context.SaveChangesAsync();
        }

        public static async Task RemovePatientMedicine(ChangePatientMedicineDTO patientMedicineDTO)
        {
            using var context = new InfoContext();
            var patientMedicine = new PatientMedicine
            {
                PatientId = patientMedicineDTO.PatientId,
                MedicineId = patientMedicineDTO.MedicineId
            };
            context.PatientsMedicine.Remove(patientMedicine);

            Patient patient = await context.Patients.Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                                                    .FirstAsync(p => p.Id == patientMedicineDTO.PatientId);
            var newPatientHistory = new History()
            {
                PatientId = patient.Id,
                PatientData = patient.ToString(),
                Timestamp = DateTime.UtcNow,
                Note = patientMedicineDTO.Note,
                Change = HistoryChange.MedicineRemoved
            };
            context.History.Add(newPatientHistory);
            await context.SaveChangesAsync();
        }

        public static void ArchivePatients()
        {
            using var context = new InfoContext();
            var patientsToArchive = context.Patients.Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                                                    .Include(p => p.Location)
                                                    .Include(p => p.Diagnosis)
                                                    .Where(p => DateTime.UtcNow.Year - p.BirthDate.Year >= 18).ToList();

            foreach (var patient in patientsToArchive)
            {
                if (patient.Age < 18)
                {
                    continue;
                }

                var newArchiveEntry = new ArchivedPatient
                {
                    PatientId = patient.Id,
                    Name = patient.Name,
                    MedicineView = patient?.MedicineView,
                    BirthDate = patient.BirthDate,
                    DisplaySex = patient.DisplaySex,
                    LocationView = patient.Location?.Name,
                    DiagnosisView = patient.Diagnosis?.Name
                };

                context.ArchivedPatients.Add(newArchiveEntry);
                context.Patients.Remove(patient);
            }

            context.SaveChanges();
        }

        #endregion

        #region Medicine

        public static List<Medicine> GetAllMedicine()
        {
            using var context = new InfoContext();
            return context.Medicine.AsNoTracking().ToList();
        }

        public static Task AddMedicine(Medicine newMedicine)
        {
            using var context = new InfoContext();
            newMedicine.Name = newMedicine.Name.Trim();
            context.Medicine.Add(newMedicine);
            return context.SaveChangesAsync();
        }

        public static Task UpdateMedicine(Medicine medicine)
        {
            using var context = new InfoContext();
            medicine.Name = medicine.Name.Trim();
            context.Medicine.Update(medicine);
            return context.SaveChangesAsync();
        }

        public static Task RemoveMedicine(Medicine medicine)
        {
            using var context = new InfoContext();
            context.Medicine.Remove(medicine);
            return context.SaveChangesAsync();
        }


        #endregion

        public static IEnumerable<Location> Locations
        {
            get
            {
                using var context = new InfoContext();
                return context.Locations.AsNoTracking().ToList();
            }
        }

        public static IEnumerable<Diagnosis> Diagnoses
        {
            get
            {
                using var context = new InfoContext();
                return context.Diagnoses.AsNoTracking().ToList();
            }
        }

        public static Task AddLocation(Location newLocation)
        {
            using var context = new InfoContext();
            context.Locations.Add(newLocation);
            return context.SaveChangesAsync();
        }

        public static Task UpdateLocation(Location location)
        {
            using var context = new InfoContext();
            context.Locations.Update(location);
            return context.SaveChangesAsync();
        }

        public static async Task<bool> TryRemoveLocation(Location location)
        {
            using var context = new InfoContext();
            if (context.Patients.Select(p => p.LocationId).Contains(location.Id))
            {
                return false;
            }

            context.Locations.Remove(location);
            await context.SaveChangesAsync();
            return true;
        }

        public static Task AddDiagnosis(Diagnosis newDiagnosis)
        {
            using var context = new InfoContext();
            newDiagnosis.Name = newDiagnosis.Name.Trim();
            context.Diagnoses.Add(newDiagnosis);
            return context.SaveChangesAsync();
        }

        public static Task UpdateDiagnosis(Diagnosis diagnosis)
        {
            using var context = new InfoContext();
            context.Diagnoses.Update(diagnosis);
            return context.SaveChangesAsync();
        }

        public static async Task<bool> RemoveDiagnosis(Diagnosis diagnosis)
        {
            using var context = new InfoContext();
            if (context.Patients.Select(p => p.DiagnosisId).Contains(diagnosis.Id))
            {
                return false;
            }

            context.Diagnoses.Remove(diagnosis);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
