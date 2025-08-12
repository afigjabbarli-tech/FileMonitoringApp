using System.ComponentModel;

namespace FileMonitoring.Core.Models
{
    public class LoaderOption : INotifyPropertyChanged
    {
        private bool _isActive;
        public string Name { get; set; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
