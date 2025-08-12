using FileMonitoring.Core.Interfaces;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class FileMonitoringService : IFileMonitoringService, IDisposable
    {
        private Timer _timer;
        private string _directory_path;
        private TimeSpan _interval;
        private HashSet<string> _existing_processed_files;
        private readonly object _lock = new();
        private bool _isChecking = false;

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

        //private bool IsProcessedFile(string file)
        //{
        //    if (string.IsNullOrEmpty(file)) return false;

        //    lock (_lock)
        //    {
        //        return _existing_processed_files.Contains(file);
        //    }
        //}

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

            lock (_lock)
            {
                _directory_path = directoryPath;
                _interval = interval;
                _existing_processed_files.Clear();
            }

            _timer.Change(TimeSpan.Zero, _interval);
        }

        private void CheckDirectory(object? state)
        {
            if (_isChecking) return;

            _isChecking = true;
            try
            {
                if (!IsValidPath(_directory_path)) return;
                if (!Directory.Exists(_directory_path)) return;

                var filesInCurrentDirectory = Directory.GetFiles(_directory_path, "*", SearchOption.TopDirectoryOnly);

                List<string> newFiles = new();

                lock (_lock)
                {
                    foreach (var file in filesInCurrentDirectory)
                    {
                        if (!_existing_processed_files.Contains(file))
                        {
                            _existing_processed_files.Add(file);
                            newFiles.Add(file);
                        }
                    }
                }

                foreach (var file in newFiles)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            NewFileDetected?.Invoke(this, file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    });
                }

            }
            finally
            {
                _isChecking = false;
            }
        }

        public void Dispose()
        {
            StopMonitoringProcess();

            lock (_lock)
            {
                _existing_processed_files?.Clear();
                NewFileDetected = null;
            }

            _timer?.Dispose();
        }
    }
}
