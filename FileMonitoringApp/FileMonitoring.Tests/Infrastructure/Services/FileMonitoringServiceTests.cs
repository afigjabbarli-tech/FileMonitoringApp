using FileMonitoring.Infrastructure.Implementations;
using System.Reflection;

namespace FileMonitoring.Tests.Infrastructure.Services
{
    public class FileMonitoringServiceTests
    {
        [Theory]
        [InlineData(@"C:\ValidPath", true)]
        [InlineData("", false)]
        [InlineData("?", false)]
        public void IsValidPath_ShouldReturnExpected(string path, bool expected)
        {
            var service = new FileMonitoringService(@"C:\", TimeSpan.FromSeconds(5));

            var method = typeof(FileMonitoringService)
                .GetMethod("IsValidPath", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(method); 

            bool result = (bool)method.Invoke(service, new object[] { path });

            Assert.Equal(expected, result);
        }
    }
}
