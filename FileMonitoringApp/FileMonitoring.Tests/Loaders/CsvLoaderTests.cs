using FileMonitoring.Loaders;

namespace FileMonitoring.Tests.Loaders
{
    public class CsvLoaderTests
    {
        [Fact]
        public async Task LoadAsync_ShouldReturnRecords_WhenCsvValid()
        {
            var loader = new CsvLoader();
            string tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFile, "2024-01-01,1,2,0.5,1.5,1000");

            var result = await loader.LoadAsync(tempFile);

            Assert.Single(result);
            Assert.Equal(1, result[0].Open);
        }
    }
}
