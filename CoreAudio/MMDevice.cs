using System;
using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    public class MMDevice
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
        
        internal MMDevice(IMMDevice realDevice)
        {
            _deviceInterface = realDevice;
        }
        
        private void GetAudioMeterInformation()
        {
            Marshal.ThrowExceptionForHR(_deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var result));
            _audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }
    }
}
