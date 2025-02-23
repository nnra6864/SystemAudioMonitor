using System;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
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