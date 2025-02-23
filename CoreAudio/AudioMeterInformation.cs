using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.Windows.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.Windows
{
    public class AudioMeterInformation
    {
        private readonly IAudioMeterInformation _audioMeterInformation;
        public AudioMeterInformationChannels PeakValues { get; }
        public EEndpointHardwareSupport HardwareSupport { get; }

        internal AudioMeterInformation(IAudioMeterInformation realInterface)
        {
            _audioMeterInformation = realInterface;
            Marshal.ThrowExceptionForHR(_audioMeterInformation.QueryHardwareSupport(out var hardwareSupp));
            HardwareSupport = (EEndpointHardwareSupport)hardwareSupp;
            PeakValues      = new(_audioMeterInformation);
        }

        public float MasterPeakValue()
        {
            Marshal.ThrowExceptionForHR(_audioMeterInformation.GetPeakValue(out var result));
            return result;
        }
    }
}