using FileMonitoring.Infrastructure.Implementations;
using Microsoft.Extensions.Configuration;

namespace FileMonitoring.Tests.Infrastructure.Services
{
    public class ConfigurationServiceTests
    {
        [Fact]
        public void GetDirectoryToWatch_ShouldThrow_WhenNotSet()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var service = new ConfigurationService(config);

            Assert.Throws<Exception>(() => service.GetDirectoryToWatch());
        }
    }
}
