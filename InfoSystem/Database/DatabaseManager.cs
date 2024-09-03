using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
                return context.Patients.ToList();
            }
        }

        public static IEnumerable<Medicine> Medicine
        {
            get
            {
                var context = new InfoContext();
                return context.Medicine.ToList();
            }
        }

        public static async Task AddPatient(Patient newPatient)
        {
            var context = new InfoContext();
            context.Patients.Add(newPatient);
            await context.SaveChangesAsync();
            context.Entry(newPatient).Collection(p => p.PatientMedicine)
                .Query()
                .Include(pm => pm.Medicine)
                .Load();
        }

        public static async Task UpdatePatient(Patient patient)
        {
            var context = new InfoContext();
            
            var dbPatient = context.Patients.Include(p => p.PatientMedicine).Single(p => p.Id == patient.Id);
            dbPatient.PatientMedicine = patient.PatientMedicine;
            dbPatient.Age = patient.Age;
            dbPatient.Name = patient.Name;

            await context.SaveChangesAsync();
            patient = dbPatient;
            context.Entry(patient).Collection(p => p.PatientMedicine)
                .Query()
                .Include(pm => pm.Medicine)
                .Load();
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
    }
}
