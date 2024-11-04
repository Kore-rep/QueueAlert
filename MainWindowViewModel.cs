using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QueueAlert;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly AudioService _audioService;
    private string? _selectedDevice;

    public MainWindowViewModel()
    {
        _audioService = new AudioService();
        AudioDevices = new ObservableCollection<string>(_audioService.GetAudioInputDevices());
    }

    public ObservableCollection<string> AudioDevices { get; }

    public string? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (_selectedDevice != value)
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
