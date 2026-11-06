using System.Windows;

namespace InfoSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Startup database logic
            InfoContext.InitDatabase();
            using var context = InfoContext.CreateDbContextWithTransactionAsync().GetAwaiter().GetResult();
            context.PatientsQueries.ArchiveOldPatientsAsync().GetAwaiter().GetResult();
            context.CommitAsync().GetAwaiter().GetResult();
        }
    }
}
