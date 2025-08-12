using FileMonitoring.Core.Interfaces;
using FileMonitoring.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FileMonitoring.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFileMonitoringService _file_monitoring_service;
        private readonly List<ILoader> _loaders;
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<FinancialRecord> _financial_records = new();
        public ObservableCollection<FinancialRecord> FinancialRecords
        {
            get { return _financial_records; }
            set
            {
                _financial_records = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel(IFileMonitoringService file_monitoring_service, List<ILoader> loaders)
        {
            _file_monitoring_service = file_monitoring_service;
            _loaders = loaders;

            _file_monitoring_service.NewFileDetected += OnNewFileDetected;
            _file_monitoring_service.StartMonitoringProcess();
        }

        private async void OnNewFileDetected(object? sender, string filePath)
        {
            var loader = _loaders.FirstOrDefault(l => l.CanLoad(filePath));
            if (loader == null)
                return;

            var financialRecords = await loader.LoadAsync(filePath);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var financialRecord in financialRecords)
                {
                    FinancialRecords.Add(financialRecord);
                }
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}