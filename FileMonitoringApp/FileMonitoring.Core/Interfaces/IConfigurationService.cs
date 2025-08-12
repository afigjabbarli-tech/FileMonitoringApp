namespace FileMonitoring.Core.Interfaces
{
    public interface IConfigurationService
    {
        string GetDirectoryToWatch();
        TimeSpan GetMonitoringInterval();
        void Reload();
    }
}
