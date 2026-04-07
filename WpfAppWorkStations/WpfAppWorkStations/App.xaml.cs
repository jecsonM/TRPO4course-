using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfAppWorkStations.Services;

namespace WpfAppWorkStations
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider serviceProvider = (new ServiceCollection()).ConfigureServicesForWholeProject();

            MainWindow = serviceProvider.GetRequiredService<MainWindow>();

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
