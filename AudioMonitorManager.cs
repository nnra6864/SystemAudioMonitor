using System;
using NnUtils.Modules.SystemAudioMonitor.CoreAudio;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class AudioMonitorManager : IDisposable
    {
        private AudioMonitor _audioMonitor;
        
        private static AudioMonitorManager _instance;
        public static AudioMonitorManager Instance => _instance ??= new();

        public static float Loudness => Instance._audioMonitor?.Loudness() ?? 0;
        
        public static void Start(AudioCaptureType captureType = AudioCaptureType.Default, string name = "",
            string streamName = "Output Device Monitor", string device = "@DEFAULT_MONITOR@", int volume = 65536, int rate = 44100,
            int bufferSize = 2048)
        {
            Instance._audioMonitor?.Dispose();
            
            captureType = captureType == AudioCaptureType.Default ? GetDefaultCaptureType() : captureType;
            switch (captureType)
            {
                case AudioCaptureType.PulseAudio: Instance.CapturePulseAudio(name, streamName, device, volume, rate, bufferSize); break;
                case AudioCaptureType.Windows: Instance.CaptureWindows(); break;
            }
        }

        private static AudioCaptureType GetDefaultCaptureType()
        {
#if UNITY_STANDALONE_LINUX
            return AudioCaptureType.PulseAudio;
#elif UNITY_STANDALONE
            return AudioCaptureType.Windows;
#endif
        }

        private void CapturePulseAudio(string name = "", string streamName = "Output Device Monitor", string device = "@DEFAULT_MONITOR@",
            int volume = 65536, int rate = 44100, int bufferSize = 2048)
        {
            _audioMonitor = new PulseAudioMonitor(name, streamName, device, volume, rate, bufferSize);
            _audioMonitor.Start();
        }

        private void CaptureWindows()
        {
            _audioMonitor = new WindowsAudioMonitor();
            _audioMonitor.Start();
        }

        public void Dispose()
        {
            _audioMonitor?.Dispose();
        }
    }
}