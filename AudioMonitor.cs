using System;
using System.Threading.Tasks;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class AudioMonitor : IDisposable
    {
        public float Loudness { get; protected set; }
        
        public async virtual Task Start()
        {
            
        }
        
        public virtual void Dispose()
        {
            
        }
    }
}