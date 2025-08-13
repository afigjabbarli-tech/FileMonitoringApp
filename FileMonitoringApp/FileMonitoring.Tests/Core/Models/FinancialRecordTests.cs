using FileMonitoring.Core.Models;

namespace FileMonitoring.Tests.Core.Models
{
    public class FinancialRecordTests
    {
        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            var date = DateTime.Today;
            var record = new FinancialRecord(date, 1.1m, 2.2m, 0.9m, 1.5m, 1000);

            Assert.Equal(date, record.Date);
            Assert.Equal(1.1m, record.Open);
            Assert.Equal(2.2m, record.High);
            Assert.Equal(0.9m, record.Low);
            Assert.Equal(1.5m, record.Close);
            Assert.Equal(1000, record.Volume);
        }
    }
}
