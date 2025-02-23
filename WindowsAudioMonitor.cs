using System;
using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class WindowsAudioMonitor : AudioMonitor
    {
        #region Variables
        private readonly IMMDevice _deviceInterface;
        private AudioMeterInformation _audioMeterInformation;
        /// <summary>
        /// Audio Meter Information
        /// </summary>
        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (_audioMeterInformation == null)
                    GetAudioMeterInformation();

                return _audioMeterInformation;
            }
        }
        #endregion

        #region Guids
        // ReSharper disable InconsistentNaming
        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        // ReSharper restore InconsistentNaming
        #endregion
        
        private void GetAudioMeterInformation()
        {
            Marshal.ThrowExceptionForHR(_deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var result));
            _audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }
        
        public override void Start() => Loudness = _audioMeterInformation.MasterPeakValue;
    }
}
