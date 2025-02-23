// Original Code
// https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IMMDeviceCollection.cs

using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio.Interfaces
{
    [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComImport]
    interface IMMDeviceCollection
    {
        int GetCount(out int numDevices);
        int Item(int deviceNumber, out IMMDevice device);
    }
}