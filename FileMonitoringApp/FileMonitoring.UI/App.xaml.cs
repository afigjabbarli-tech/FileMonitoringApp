using FileMonitoring.Core.Interfaces;
using FileMonitoring.Infrastructure.Implementations;
using FileMonitoring.Loaders;
using FileMonitoring.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace FileMonitoring.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            IConfigurationService configurationService = new ConfigurationService(config);

            IFileMonitoringService fileMonitoringService =
                new FileMonitoringService(configurationService.GetDirectoryToWatch(), configurationService.GetMonitoringInterval());

            IPluginLoaderService pluginLoaderService = new PluginLoaderService();

            var mainViewModel = new MainViewModel(fileMonitoringService, pluginLoaderService, configurationService);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();
        }
    }
}
