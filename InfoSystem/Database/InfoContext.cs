﻿using Microsoft.EntityFrameworkCore;

namespace InfoSystem
{
    internal class InfoContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medicine> Medicine {  get; set; }

        private const string DbPath = "infosystem.db";
        public InfoContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");

        public static void InitDatabase()
        {
            var context = new InfoContext();
            context.Database.EnsureCreated();
        }
    }
}
