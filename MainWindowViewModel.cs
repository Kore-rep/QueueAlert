using NAudio.CoreAudioApi;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QueueAlert;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly AudioService _audioService;
    private MMDevice? _selectedDevice = null;
    private bool _isListening = false;

    public MainWindowViewModel()
    {
        _audioService = new AudioService();
        AudioDevices = new ObservableCollection<MMDevice>(_audioService.GetAudioOutputDevices());
    }

    public ObservableCollection<MMDevice> AudioDevices { get; }

    public MMDevice? SelectedDevice
    {
        get => _audioService.SelectedDevice;
        set
        {
            if (_selectedDevice != value)
            {
                _audioService.SelectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }
    }

    public bool IsListening
    {
        get => _isListening;
        private set
        {
            if (_isListening != value)
            {
                _isListening = value;
                OnPropertyChanged(nameof(IsListening));
            }
        }
    }

    public async void StartListening()
    {
        IsListening = true;
        _audioService.ListenToAudioOutput();

    }

    public async void StopListening()
    {
        IsListening = false;
        _audioService.StopListening();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
