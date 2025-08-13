using FileMonitoring.Infrastructure.Implementations;

namespace FileMonitoring.Tests.Infrastructure.Services
{
    public class PluginLoaderServiceTests
    {
        [Fact]
        public void InstallPluginsFromCurrentDirectory_ShouldReturnEmpty_WhenFolderDoesNotExist()
        {
            var service = new PluginLoaderService();
            var result = service.InstallPluginsFromCurrentDirectory("NonExistingFolder");

            Assert.Empty(result);
        }
    }
}
