﻿using System.Linq;
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

        public static async Task<Patient> AddPatient(CreatePatientDTO createPatient)
        {
            var context = new InfoContext();
            var patient = new Patient
            {
                Id = createPatient.Id,
                Name = createPatient.Name,
                Age = createPatient.Age,
                Medicine = [.. context.Medicine.Where(m => createPatient.MedicineIds.Contains(m.Id))],
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
            patient.Age = updatePatient.Age;
            patient.Medicine = [.. context.Medicine.Where(m => updatePatient.MedicineIds.Contains(m.Id))];

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
    }
}
