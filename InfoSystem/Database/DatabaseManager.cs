using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace InfoSystem
{
    internal static class DatabaseManager
    {
        public static int LastPatientId()
        {
            var context = new InfoContext();
            var lastId = context.Patients.OrderByDescending(id => id).FirstOrDefault()?.Id;

            return lastId ?? -1;
        }

        public static IEnumerable<Patient> Patients
        {
            get
            {
                var context = new InfoContext();
                return context.Patients.AsNoTracking().ToList();
            }
        }

        public static IEnumerable<Medicine> Medicine
        {
            get
            {
                var context = new InfoContext();
                return context.Medicine.AsNoTracking().ToList();
            }
        }
        public static IEnumerable<Location> Locations
        {
            get
            {
                var context = new InfoContext();
                return context.Locations.AsNoTracking().ToList();
            }
        }

        public static IEnumerable<Diagnosis> Diagnoses
        {
            get
            {
                var context = new InfoContext();
                return context.Diagnoses.AsNoTracking().ToList();
            }
        }

        public static async Task<Patient> AddPatient(CreatePatientDTO createPatient)
        {
            var context = new InfoContext();
            var patient = new Patient
            {
                Name = createPatient.Name,
                BirthDate = createPatient.BirthDate,
                Gender = createPatient.Gender,
                Medicine = [.. context.Medicine.Where(m => createPatient.MedicineIds.Contains(m.Id))],
                Location = context.Locations.First(l => l.Id == createPatient.LocationId),
                Diagnosis = context.Diagnoses.First(d => d.Id == createPatient.DiagnosisId)
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            return patient;
        }

        public static async Task<Patient> UpdatePatient(CreatePatientDTO updatePatient)
        {
            var context = new InfoContext();

            var patient = context.Patients.Include(p => p.Medicine).Single(p => p.Id == updatePatient.Id);
            patient.Name = updatePatient.Name;
            patient.BirthDate = updatePatient.BirthDate;
            patient.Gender = updatePatient.Gender;
            patient.Location = context.Locations.First(l => l.Id == updatePatient.LocationId);
            patient.Medicine = [.. context.Medicine.Where(m => updatePatient.MedicineIds.Contains(m.Id))];
            patient.Diagnosis = context.Diagnoses.First(d => d.Id == updatePatient.DiagnosisId);

            await context.SaveChangesAsync();

            return patient;
        }

        public static Task RemovePatient(Patient patient)
        {
            var context = new InfoContext();
            context.Patients.Remove(patient);
            return context.SaveChangesAsync();
        }

        public static Task AddMedicine(Medicine newMedicine)
        {
            var context = new InfoContext();
            context.Medicine.Add(newMedicine);
            return context.SaveChangesAsync();
        }

        public static Task UpdateMedicine(Medicine medicine)
        {
            var context = new InfoContext();
            context.Medicine.Update(medicine);
            return context.SaveChangesAsync();
        }

        public static Task RemoveMedicine(Medicine medicine)
        {
            var context = new InfoContext();
            context.Medicine.Remove(medicine);
            return context.SaveChangesAsync();
        }

        public static Task AddLocation(Location newLocation)
        {
            var context = new InfoContext();
            context.Locations.Add(newLocation);
            return context.SaveChangesAsync();
        }

        public static Task UpdateLocation(Location location)
        {
            var context = new InfoContext();
            context.Locations.Update(location);
            return context.SaveChangesAsync();
        }

        public static async Task<bool> TryRemoveLocation(Location location)
        {
            var context = new InfoContext();
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
            var context = new InfoContext();
            context.Diagnoses.Add(newDiagnosis);
            return context.SaveChangesAsync();
        }

        public static Task UpdateDiagnosis(Diagnosis diagnosis)
        {
            var context = new InfoContext();
            context.Diagnoses.Update(diagnosis);
            return context.SaveChangesAsync();
        }

        public static async Task<bool> RemoveDiagnosis(Diagnosis diagnosis)
        {
            var context = new InfoContext();
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
