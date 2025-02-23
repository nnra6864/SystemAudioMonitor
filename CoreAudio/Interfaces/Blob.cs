// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/Blob.cs

using System;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces
{
    /// <summary>
    /// Representation of binary large object container.
    /// </summary>
    public struct Blob
    {
        /// <summary>
        /// Length of binary object.
        /// </summary>
        public int Length;
        
        /// <summary>
        /// Pointer to buffer storing data.
        /// </summary>
        public IntPtr Data;
    }
}