using System;
using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// Audio Meter Information Channels
    /// </summary>
    public class AudioMeterInformationChannels
    {
        private readonly IAudioMeterInformation _audioMeterInformation;

        /// <summary>
        /// Metering Channel Count
        /// </summary>
        public int Count
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioMeterInformation.GetMeteringChannelCount(out var result));
                return result;
            }
        }

        /// <summary>
        /// Get Peak value
        /// </summary>
        /// <param name="index">Channel index</param>
        /// <returns>Peak value</returns>
        public float this[int index]
        {
            get
            {
                var channels = Count;
                if (index >= channels)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        $"Peak index cannot be greater than number of channels ({channels})");
                }

                var peakValues = new float[Count];
                var p = GCHandle.Alloc(peakValues, GCHandleType.Pinned);
                Marshal.ThrowExceptionForHR(_audioMeterInformation.GetChannelsPeakValues(peakValues.Length, p.AddrOfPinnedObject()));
                p.Free();
                return peakValues[index];
            }
        }

        internal AudioMeterInformationChannels(IAudioMeterInformation parent) => _audioMeterInformation = parent;
    }
}