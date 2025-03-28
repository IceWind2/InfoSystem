using Microsoft.EntityFrameworkCore;

namespace InfoSystem
{
    internal class InfoContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicine> PatientsMedicine{ get; set; }
        public DbSet<Medicine> Medicine {  get; set; }
        public DbSet<Location> Locations{  get; set; }
        public DbSet<Diagnosis> Diagnoses{  get; set; }
        public DbSet<History> History{  get; set; }

        private const string DbPath = "infosystem.db";
        public InfoContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public static void InitDatabase()
        {
            var context = new InfoContext();
            context.Database.EnsureCreated();
        }
    }
}
