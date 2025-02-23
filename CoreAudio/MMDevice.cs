// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/MMDevice.cs

using System;
using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    public class MMDevice
    {
        // ReSharper disable InconsistentNaming
        private static Guid IID_IAudioMeterInformation = new("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        // ReSharper restore InconsistentNaming
        
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
        
        internal MMDevice(IMMDevice realDevice) => _deviceInterface = realDevice;

        private void GetAudioMeterInformation()
        {
            Marshal.ThrowExceptionForHR(_deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var result));
            _audioMeterInformation = new(result as IAudioMeterInformation);
        }
    }
}
