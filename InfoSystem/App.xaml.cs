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
            DatabaseManager.ArchivePatients();
        }
    }
}
