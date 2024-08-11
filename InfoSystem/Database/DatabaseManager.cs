using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static Task AddPatient(Patient newPatient)
        {
            var context = new InfoContext();
            context.Patients.Add(newPatient);
            return context.SaveChangesAsync();
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

        public static Task RemoveMedicine(Medicine medicine)
        {
            var context = new InfoContext();
            context.Medicine.Remove(medicine);
            return context.SaveChangesAsync();
        }
    }
}
