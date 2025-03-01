// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/PropertyKey.cs

using System;
// ReSharper disable InconsistentNaming

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// PROPERTYKEY is defined in wtypes.h
    /// </summary>
    public struct PropertyKey
    {
        /// <summary>
        /// Format ID
        /// </summary>
        public Guid formatId;
        
        /// <summary>
        /// Property ID
        /// </summary>
        public int propertyId;
        
        /// <summary>
        /// <param name="formatId"></param>
        /// <param name="propertyId"></param>
        /// </summary>
        public PropertyKey(Guid formatId, int propertyId)
        {
            this.formatId   = formatId;
            this.propertyId = propertyId;
        }
    }
}