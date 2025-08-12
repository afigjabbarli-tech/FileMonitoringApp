using FileMonitoring.Core.Interfaces;
using FileMonitoring.Core.Models;
using FileMonitoring.Loaders;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FileMonitoring.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFileMonitoringService _fileMonitoringService;
        private readonly IPluginLoaderService _pluginLoaderService;
        private readonly IConfigurationService _configurationService;

        private List<ILoader> _allLoaders = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly SemaphoreSlim _semaphore = new(8);

        public ObservableCollection<FinancialRecord> FinancialRecords { get; set; } = new();
        public ObservableCollection<LoaderOption> AvailableLoaders { get; set; } = new();

        private string _watchDirectory;
        public string WatchDirectory
        {
            get => _watchDirectory;
            set { _watchDirectory = value; OnPropertyChanged(); }
        }

        private int _intervalSeconds;
        public int IntervalSeconds
        {
            get => _intervalSeconds;
            set { _intervalSeconds = value; OnPropertyChanged(); }
        }

        private string _pluginDirectory;
        public string PluginDirectory
        {
            get => _pluginDirectory;
            set { _pluginDirectory = value; OnPropertyChanged(); }
        }

        public ICommand SaveConfigCommand { get; }

        public MainViewModel(
            IFileMonitoringService fileMonitoringService,
            IPluginLoaderService pluginLoaderService,
            IConfigurationService configurationService)
        {
            _fileMonitoringService = fileMonitoringService;
            _pluginLoaderService = pluginLoaderService;
            _configurationService = configurationService;

            SaveConfigCommand = new RelayCommand(_ => SaveConfig());

            LoadConfig();

            _fileMonitoringService.NewFileDetected += OnNewFileDetected;
            _fileMonitoringService.StartMonitoringProcess();
        }

        private void LoadConfig()
        {
            WatchDirectory = _configurationService.GetDirectoryToWatch();
            IntervalSeconds = (int)_configurationService.GetMonitoringInterval().TotalSeconds;
            PluginDirectory = _configurationService.GetPluginDirectory();

            var pluginLoaders = _pluginLoaderService.InstallPluginsFromCurrentDirectory(PluginDirectory);

            pluginLoaders.Add(new CsvLoader());
            pluginLoaders.Add(new TxtLoader());
            pluginLoaders.Add(new XmlLoader());

            var activeNames = _configurationService.GetActiveLoaders();

            AvailableLoaders.Clear();
            foreach (var loader in pluginLoaders)
            {
                AvailableLoaders.Add(new LoaderOption
                {
                    Name = loader.GetType().Name,
                    IsActive = activeNames.Contains(loader.GetType().Name)
                });
            }

            _allLoaders = pluginLoaders.Where(l => activeNames.Contains(l.GetType().Name)).ToList();
        }

        public void SaveConfig()
        {
            var active = AvailableLoaders.Where(l => l.IsActive).Select(l => l.Name).ToList();
            _configurationService.UpdateSettings(WatchDirectory, IntervalSeconds, PluginDirectory, active);

            _fileMonitoringService.UpdateMonitoringSettings(WatchDirectory, TimeSpan.FromSeconds(IntervalSeconds));
            LoadConfig();
        }

        private void OnNewFileDetected(object? sender, string filePath)
        {
            _ = Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    var loader = _allLoaders.FirstOrDefault(l => l.CanLoad(filePath));
                    if (loader == null)
                        return;

                    var records = await loader.LoadAsync(filePath);

                    await Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        foreach (var record in records)
                            FinancialRecords.Add(record);
                    });
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
