using NnUtils.Modules.SystemAudioMonitor.Windows;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class WindowsAudioMonitor : AudioMonitor
    {
        private MMDevice _device;
        
        public override void Start()
        {
            var enumerator = new MMDeviceEnumerator();
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            Loudness = _device.AudioMeterInformation.MasterPeakValue;
        }

        public override void Dispose()
        {
            _device = null;
        }
    }
}