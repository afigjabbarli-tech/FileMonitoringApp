using FileMonitoring.Core.Interfaces;
using FileMonitoring.Core.Models;
using System.Globalization;

namespace FileMonitoring.Loaders
{
    public class TxtLoader : ILoader
    {
        public bool CanLoad(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            return Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<FinancialRecord>> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var financialRecords = new List<FinancialRecord>();

            if (!File.Exists(filePath))
                return financialRecords;

            using var reader = new StreamReader(filePath);

            var header = await reader.ReadLineAsync(cancellationToken);

            string? line;

            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parts = line.Split(';')
                                .Select(p => p.Trim())
                                .ToArray();

                if (parts.Length != 6)
                    continue;

                if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    continue;

                if (!decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var open))
                    continue;

                if (!decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var high))
                    continue;

                if (!decimal.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var low))
                    continue;

                if (!decimal.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var close))
                    continue;

                if (!long.TryParse(parts[5], NumberStyles.Any, CultureInfo.InvariantCulture, out var volume))
                    continue;

                var financialRecord = new FinancialRecord(date, open, high, low, close, volume);

                financialRecords.Add(financialRecord);
            }

            return financialRecords;
        }
    }
}
