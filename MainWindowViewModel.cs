using NAudio.CoreAudioApi;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QueueAlert;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly AudioService _audioService;
    private MMDevice? _selectedDevice;

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
            Console.WriteLine("HERE");
            if (_selectedDevice != value)
            {
                _audioService.SelectedDevice = value;
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
