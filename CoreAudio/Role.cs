// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Role.cs

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// The ERole enumeration defines constants that indicate the role 
    /// that the system has assigned to an audio endpoint device
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Games, system notification sounds, and voice commands.
        /// </summary>
        Console,

        /// <summary>
        /// Music, movies, narration, and live music recording
        /// </summary>
        Multimedia,

        /// <summary>
        /// Voice communications (talking to another person).
        /// </summary>
        Communications,
    }
}