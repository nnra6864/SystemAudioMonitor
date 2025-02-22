namespace NnUtils.Modules.SystemAudioMonitor
{
    public class AudioMonitorManager
    {
        private AudioMonitor _audioMonitor;

        public float Loudness => _audioMonitor?.Loudness ?? 0;
        
        public void Start(AudioCaptureType captureType = AudioCaptureType.Default, string name = "",
            string streamName = "Output Device Monitor", string device = "@DEFAULT_MONITOR@", int volume = 65536, int rate = 44100,
            int bufferSize = 2048, int updateInterval = 50)
        {
            _audioMonitor?.Dispose();
            
            captureType = captureType == AudioCaptureType.Default ? GetDefaultCaptureType() : captureType;
            switch (captureType)
            {
                case AudioCaptureType.PulseAudio: CapturePulseAudio(name, streamName, device, volume, rate, bufferSize); break;
                case AudioCaptureType.Windows: CaptureWindows(updateInterval); break;
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
            _             = _audioMonitor.Start();
        }

        private void CaptureWindows(int updateInterval = 50)
        {
            _audioMonitor = new WindowsAudioMonitor(updateInterval);
            _             = _audioMonitor.Start();
        }

        public void Dispose()
        {
            _audioMonitor?.Dispose();
        }
    }
}