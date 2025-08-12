namespace FileMonitoring.Core.Interfaces
{
    public interface IConfigurationService
    {
        List<string> GetActiveLoaders();
        string GetPluginDirectory();
        void UpdateSettings(string watchDir, int intervalSec, string pluginDir, List<string> activeLoaders);
        string GetDirectoryToWatch();
        TimeSpan GetMonitoringInterval();
        void Reload();
    }
}
