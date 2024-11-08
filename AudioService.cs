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
    private static WasapiLoopbackCapture capture;
    private static BlockingCollection<AudioSamples> realtimeSource;
    private static MMDevice _selectedDevice;
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
        return [.. _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)];
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
        realtimeSource = new BlockingCollection<AudioSamples>();
        StreamAudioOutput();
    }

    public bool IsListening()
    {
        return capture.CaptureState == CaptureState.Starting || capture.CaptureState == CaptureState.Capturing;
    }

    static void StreamAudioOutput()
    {
        capture = new WasapiLoopbackCapture(_selectedDevice);
        capture.DataAvailable += (_, e) =>
        {
            // using short because 16 bits per sample is used as input wave format
            short[] samples = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, samples, 0, e.BytesRecorded);
            // converting to [-1, +1] range
            float[] floats = Array.ConvertAll(samples, (sample => (float)sample / short.MaxValue));
            var sams = new AudioSamples(floats, string.Empty, _sampleRate);
            realtimeSource.Add(sams);
        };
        capture.RecordingStopped += (_, _) => Console.WriteLine("Recording stopped.");
        capture.StartRecording();
    }

    public void StopListening()
    {
        capture.StopRecording();
        capture.Dispose();
        Console.WriteLine(realtimeSource.ToString());
    }
}
