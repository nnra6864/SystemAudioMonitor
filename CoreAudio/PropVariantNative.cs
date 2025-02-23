// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/PropVariantNative.cs

using System;
using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    class PropVariantNative
    {
        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(ref PropVariant pvar);

        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(IntPtr pvar);
    }
}