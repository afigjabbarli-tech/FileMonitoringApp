using System.Diagnostics;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class FileWatcherService
    {
        private Timer _timer;
        private string _directory_path;
        private TimeSpan _interval;
        private HashSet<string> _existing_processed_files;

        public event EventHandler<string> NewFileDetected;

        public FileWatcherService(string directoryPath, TimeSpan interval)
        {
            _directory_path = directoryPath;
            _interval = interval;
            _timer = new Timer(CheckDirectory, null, Timeout.Infinite, Timeout.Infinite);
        }
        private bool IsProcessedFile(string file)
        {
            if(string.IsNullOrEmpty(file)) return false;
            return _existing_processed_files.Contains(file);
        }
        private void CheckDirectory(object? state)
        {

        }
    }
}
