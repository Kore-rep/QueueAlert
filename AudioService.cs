using System.Collections.Generic;
using System.Data;
using System.Linq;
using NAudio.CoreAudioApi;

public class AudioService
{
    private readonly MMDeviceEnumerator _deviceEnumerator;

    public AudioService()
    {
        _deviceEnumerator = new MMDeviceEnumerator();
    }

    public List<string> GetAudioInputDevices()
    {
        return _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                                 .Select(device => device.FriendlyName)
                                 .ToList();
    }
}
