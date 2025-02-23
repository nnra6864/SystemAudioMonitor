// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/StorageAccessMode.cs

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces
{
    /// <summary>
    /// MMDevice STGM enumeration
    /// </summary>
    public enum StorageAccessMode
    {
        /// <summary>
        /// Read-only access mode.
        /// </summary>
        Read,
        
        /// <summary>
        /// Write-only access mode.
        /// </summary>
        Write,
        
        /// <summary>
        /// Read-write access mode.
        /// </summary>
        ReadWrite
    }
}