using FileMonitoring.Core.Interfaces;
using FileMonitoring.Core.Models;
using System.Globalization;
using System.Xml.Linq;

namespace FileMonitoring.Loaders
{
    public class XmlLoader : ILoader
    {
        public bool CanLoad(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            return Path.GetExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<FinancialRecord>> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var financialRecords = new List<FinancialRecord>();

            if (!File.Exists(filePath))
                return financialRecords;

            var xmlContent = await File.ReadAllTextAsync(filePath, cancellationToken);

            var doc = XDocument.Parse(xmlContent);

            var values = doc.Descendants("value");

            foreach (var value in values)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dateStr = (string?)value.Element("date");
                var openStr = (string?)value.Element("open");
                var highStr = (string?)value.Element("high");
                var lowStr = (string?)value.Element("low");
                var closeStr = (string?)value.Element("close");
                var volumeStr = (string?)value.Element("volume");

                if (dateStr == null || openStr == null || highStr == null || lowStr == null || closeStr == null || volumeStr == null)
                    continue;

                if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    continue;

                if (!decimal.TryParse(openStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var open))
                    continue;

                if (!decimal.TryParse(highStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var high))
                    continue;

                if (!decimal.TryParse(lowStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var low))
                    continue;

                if (!decimal.TryParse(closeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var close))
                    continue;

                if (!long.TryParse(volumeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var volume))
                    continue;

                var financialRecord = new FinancialRecord(date, open, high, low, close, volume);

                financialRecords.Add(financialRecord);
            }

            return financialRecords;
        }
    }
}
