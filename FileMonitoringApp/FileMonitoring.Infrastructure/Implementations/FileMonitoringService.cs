using FileMonitoring.Core.Interfaces;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class FileMonitoringService : IFileMonitoringService, IDisposable
    {
        private Timer _timer;
        private string _directory_path;
        private TimeSpan _interval;
        private HashSet<string> _existing_processed_files;

        public event EventHandler<string> NewFileDetected;

        public FileMonitoringService(string directoryPath, TimeSpan interval)
        {
            if (!IsValidPath(directoryPath))
                throw new ArgumentException("Invalid directory path", nameof(directoryPath));

            _directory_path = directoryPath;
            _interval = interval;
            _existing_processed_files = new HashSet<string>();
            _timer = new Timer(CheckDirectory, null, Timeout.Infinite, Timeout.Infinite);
        }

        private bool IsProcessedFile(string file)
        {
            if (string.IsNullOrEmpty(file)) return false;
            return _existing_processed_files.Contains(file);
        }

        private bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (path.Length >= 260)
                return false;

            char[] invalidChars = Path.GetInvalidPathChars();
            if (path.Any(c => invalidChars.Contains(c)))
                return false;

            try
            {
                Path.GetFullPath(path);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void StartMonitoringProcess() => _timer.Change(TimeSpan.Zero, _interval);

        public void StopMonitoringProcess() => _timer.Change(Timeout.Infinite, Timeout.Infinite);

        public void UpdateMonitoringSettings(string directoryPath, TimeSpan interval)
        {
            if (!IsValidPath(directoryPath))
                throw new ArgumentException("Invalid directory path", nameof(directoryPath));

            _directory_path = directoryPath;
            _interval = interval;
            _timer.Change(TimeSpan.Zero, _interval);
            _existing_processed_files.Clear();
        }

        private void CheckDirectory(object? state)
        {
            if (!IsValidPath(_directory_path)) return;

            if (!Directory.Exists(_directory_path)) return;

            var filesInCurrentDirectory = Directory
                .GetFiles(_directory_path, "*", SearchOption.TopDirectoryOnly);

            foreach (var fileInCurrentDirectory in filesInCurrentDirectory)
            {
                if (IsProcessedFile(fileInCurrentDirectory)) continue;

                _existing_processed_files.Add(fileInCurrentDirectory);

                NewFileDetected?.Invoke(this, fileInCurrentDirectory);
            }
        }

        public void Dispose()
        {
            StopMonitoringProcess();     
            _timer?.Dispose();

            NewFileDetected = null; 
            _existing_processed_files?.Clear();
        }
    }
}
