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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(8);

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

        private void OnNewFileDetected(object? sender, string filePath)
        {
            _ = Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    var loader = _loaders.FirstOrDefault(l => l.CanLoad(filePath));
                    if (loader == null)
                        return;

                    List<FinancialRecord> financialRecords;
                    try
                    {
                        financialRecords = await loader.LoadAsync(filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }

                    try
                    {
                        await Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var record in financialRecords)
                                FinancialRecords.Add(record);
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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