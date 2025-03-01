// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IPropertyStore.cs

using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces
{
    /// <summary>
    /// is defined in propsys.h
    /// </summary>
    [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComImport]
    interface IPropertyStore
    {
        int GetCount(out int propCount);
        int GetAt(int property, out PropertyKey key);
        int GetValue(ref PropertyKey key, out PropVariant value);
        int SetValue(ref PropertyKey key, ref PropVariant value);
        int Commit();
    }
}