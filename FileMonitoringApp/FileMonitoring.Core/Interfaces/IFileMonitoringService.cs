namespace FileMonitoring.Core.Interfaces
{
    public interface IFileMonitoringService
    {
        event EventHandler<string> NewFileDetected;
        void StartMonitoringProcess();
        void StopMonitoringProcess();
        void UpdateMonitoringSettings(string directoryPath, TimeSpan interval);
    }
}
