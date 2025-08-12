using FileMonitoring.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class ConfigurationService : IConfigurationService
    {
        private IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetDirectoryToWatch()
        {
            var dir = _configuration["WatchDirectory"];
            if (string.IsNullOrEmpty(dir))
                throw new Exception("InputDirectory not set");
            return dir;
        }

        public TimeSpan GetMonitoringInterval()
        {
            var secondsStr = _configuration["MonitoringIntervalSeconds"] ?? "5";
            if (!int.TryParse(secondsStr, out int seconds))
                seconds = 5;
            return TimeSpan.FromSeconds(seconds);
        }

        public void Reload()
        {
          
        }
    }
}
