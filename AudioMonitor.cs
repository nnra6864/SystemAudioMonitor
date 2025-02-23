using System;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class AudioMonitor : IDisposable
    {
        public Func<float> Loudness { get; protected set; }
        
        public virtual void Start()
        {
            
        }
        
        public virtual void Dispose()
        {
            
        }
    }
}