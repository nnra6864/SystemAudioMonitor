// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/EEndpointHardwareSupport.cs

using System;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// Endpoint Hardware Support
    /// </summary>
    [Flags]
    public enum EEndpointHardwareSupport
    {
        /// <summary>
        /// Volume
        /// </summary>
        Volume = 0x00000001,
     
        /// <summary>
        /// Mute
        /// </summary>
        Mute = 0x00000002,
        
        /// <summary>
        /// Meter
        /// </summary>
        Meter = 0x00000004
    }
}