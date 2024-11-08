using NAudio.CoreAudioApi;
using NAudio.Wave;
using SoundFingerprinting.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class AudioService
{
    private readonly MMDeviceEnumerator _deviceEnumerator;
    private static readonly int _sampleRate = 5512;
    private static WaveInEvent waveSource;
    private static BlockingCollection<AudioSamples> realtimeSource;
    private MMDevice _selectedDevice;
    public MMDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            _selectedDevice = value;
        }
    }

    public AudioService()
    {
        _deviceEnumerator = new MMDeviceEnumerator();
    }

    public List<MMDevice> GetAudioOutputDevices()
    {
        return [.. _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)];
    }

    public void SelectAudioOutput(string deviceName)
    {
        SelectedDevice = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).First(device => device.FriendlyName == deviceName);
    }

    /**
     * This function should start a thread that listens to the Audio and streams it to a BlockingCollection and then start matching from that collection
     * using SoundFingerprinting's Realtime Query service.
     * <see cref="https://github.com/AddictedCS/soundfingerprinting/wiki/Realtime-Query-Command"/>
     */
    public void ListenToAudioOutput()
    {
        return;
        //_ = Task.Factory.StartNew();
    }

    static void StreamAudioOutput()
    {
        var waveSource = new WaveInEvent();
        waveSource.DeviceNumber = 0;
        waveSource.WaveFormat = new NAudio.Wave.WaveFormat(rate: _sampleRate, bits: 16, channels: 1);
        waveSource.DataAvailable += (_, e) =>
        {
            // using short because 16 bits per sample is used as input wave format
            short[] samples = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, samples, 0, e.BytesRecorded);
            // converting to [-1, +1] range
            float[] floats = Array.ConvertAll(samples, (sample => (float)sample / short.MaxValue));
            realtimeSource.Add(new AudioSamples(floats, string.Empty, _sampleRate));
        };
        waveSource.RecordingStopped += (_, _) => Console.WriteLine("Recording stopped.");
        waveSource.BufferMilliseconds = 1000;
        waveSource.StartRecording();

    }
}
