using FileMonitoring.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class ConfigurationService : IConfigurationService
    {
        private IConfiguration _configuration;
        private readonly string _filePath;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _filePath = "appsettings.json";
        }

        public string GetDirectoryToWatch()
        {
            var dir = _configuration["WatchDirectory"];
            if (string.IsNullOrEmpty(dir))
                throw new Exception("WatchDirectory not set");
            return dir;
        }

        public TimeSpan GetMonitoringInterval()
        {
            var secondsStr = _configuration["MonitoringIntervalSeconds"] ?? "5";
            if (!int.TryParse(secondsStr, out int seconds))
                seconds = 5;
            return TimeSpan.FromSeconds(seconds);
        }

        public string GetPluginDirectory() => _configuration["PluginDirectory"] ?? "Plugins";

        public List<string> GetActiveLoaders()
        {
            var section = _configuration.GetSection("ActiveLoaders");
            return section.Exists()
                ? section.GetChildren().Select(s => s.Value).ToList()
                : new List<string>();
        }

        public void UpdateSettings(string watchDir, int intervalSec, string pluginDir, List<string> activeLoaders)
        {
            var json = File.ReadAllText(_filePath);
            var doc = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            doc["WatchDirectory"] = watchDir;
            doc["MonitoringIntervalSeconds"] = intervalSec;
            doc["PluginDirectory"] = pluginDir;
            doc["ActiveLoaders"] = activeLoaders;

            var newJson = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, newJson);

            Reload();
        }

        public void Reload()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }
    }
}
