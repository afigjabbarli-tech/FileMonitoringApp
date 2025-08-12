using FileMonitoring.Core.Models;

namespace FileMonitoring.Core.Interfaces
{
    public interface ILoader
    {
        bool CanLoad(string filePath);
        Task<List<FinancialRecord>> LoadAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
