using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSystem
{
    internal class InfoContext : DbContext
    {
        public static InfoContext CreateDbContext()
        {
            return new InfoContext(new PatientsQueries());
        }

        public static async Task<InfoContext> CreateDbContextWithTransactionAsync()
        {
            var context = new InfoContext(new PatientsQueries());
            await context.BeginTransactionAsync();
            return context;
        }

        public static void InitDatabase()
        {
            using var context = new InfoContext(null);
            context.Database.EnsureCreated();
        }

        private IDbContextTransaction? _dbTransaction;
        private const string DbPath = "infosystem.db";

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicine> PatientsMedicine { get; set; }
        public DbSet<Medicine> Medicine { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<ArchivedPatient> ArchivedPatients { get; set; }

        public IPatientsQueries PatientsQueries { get; set; }

        protected InfoContext(IPatientsQueries patientsQueries)
        {
            PatientsQueries = patientsQueries;
            PatientsQueries?.SetDbContext(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public async Task BeginTransactionAsync()
        {
            if (_dbTransaction != null)
            {
                throw new InvalidOperationException("Can not begin transaction: transaction has already started");
            }

            _dbTransaction = await this.Database.BeginTransactionAsync();
        }

        public async Task<bool> CommitAsync()
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("Can not commit transaction: transaction was not started");
            }

            try
            {
                await SaveChangesAsync();
                await _dbTransaction.CommitAsync();

                return true;
            }
            catch
            {
                // log
                return false;
            }
            finally
            {
                _dbTransaction.Dispose();
                _dbTransaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("Can not rollback transaction: transaction was not started");
            }

            await _dbTransaction.RollbackAsync();
            _dbTransaction.Dispose();
            _dbTransaction = null;
        }

        public void AddEntity<T>(T entity) where T : class
        {
            this.Set<T>().Add(entity);
        }

        public List<T> GetAllEntities<T>() where T : class
        {
            return this.Set<T>().AsNoTracking().ToList();
        }

        public Task<List<T>> GetAllEntitiesAsync<T>() where T : class
        {
            return this.Set<T>().AsNoTracking().ToListAsync();
        }

        public void UpdateEntity<T>(T entity) where T : class
        {
            this.Entry<T>(entity).State = EntityState.Modified;
            this.Set<T>().Update(entity);
        }
        
        public void RemoveEntity<T>(T entity) where T : class
        {
            this.Set<T>().Remove(entity);
        }
    }
}
