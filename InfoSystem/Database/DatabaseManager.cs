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
    }
}
