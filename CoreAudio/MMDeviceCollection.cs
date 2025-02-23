using System.Collections.Generic;
using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// Multimedia Device Collection
    /// </summary>
    public class MMDeviceCollection : IEnumerable<MMDevice>
    {
        private readonly IMMDeviceCollection _mmDeviceCollection;

        /// <summary>
        /// Device count
        /// </summary>
        public int Count
        {
            get
            {
                Marshal.ThrowExceptionForHR(_mmDeviceCollection.GetCount(out var result));
                return result;
            }
        }

        /// <summary>
        /// Get device by index
        /// </summary>
        /// <param name="index">Device index</param>
        /// <returns>Device at the specified index</returns>
        public MMDevice this[int index]
        {
            get
            {
                _mmDeviceCollection.Item(index, out var result);
                return new MMDevice(result);
            }
        }

        internal MMDeviceCollection(IMMDeviceCollection parent)
        {
            _mmDeviceCollection = parent;
        }

        #region IEnumerable<MMDevice> Members

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Device enumerator</returns>
        public IEnumerator<MMDevice> GetEnumerator()
        {            
            for (int index = 0; index < Count; index++)
            {
                yield return this[index];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}