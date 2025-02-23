using System;
using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class WindowsAudioMonitor : AudioMonitor
    {
        public override void Start() => Loudness = GetCurrentLoudness;

        // COM class for device enumerator.
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumeratorComObject { }

        // IMMDeviceEnumerator interface.
        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            // Note: Other methods are omitted.
            int NotImpl1();
            [PreserveSig]
            int GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice ppDevice);
        }

        private enum DataFlow { Render, Capture, All }
        private enum Role { Console, Multimedia, Communications }

        // IMMDevice interface.
        [ComImport]
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, CLSCTX dwClsCtx, IntPtr pActivationParams, out object ppInterface);
        }

        [Flags]
        private enum CLSCTX
        {
            InprocServer = 0x1,
            InprocHandler = 0x2,
            LocalServer = 0x4,
            RemoteServer = 0x10,
            All = InprocServer | InprocHandler | LocalServer | RemoteServer
        }

        // IAudioMeterInformation interface for getting the current peak value.
        [ComImport]
        [Guid("C02216F6-5E12-4ADF-8CF2-1DBA2E5C7A1D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioMeterInformation
        {
            [PreserveSig]
            int GetPeakValue(out float pfPeak);
        }

        // Retrieves the current loudness (peak value as a float).
        public static float GetCurrentLoudness()
        {
            // Create the device enumerator.
            var enumerator = (IMMDeviceEnumerator)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")));

            // Get the default audio rendering device (multimedia role).
            IMMDevice device;
            int hr = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia, out device);
            if (hr != 0) Marshal.ThrowExceptionForHR(hr);

            // Activate the IAudioMeterInformation interface on the device.
            Guid IID_IAudioMeterInformation = new Guid("C02216F6-5E12-4ADF-8CF2-1DBA2E5C7A1D");
            object meterObj;
            hr = device.Activate(ref IID_IAudioMeterInformation, CLSCTX.All, IntPtr.Zero, out meterObj);
            if (hr != 0) Marshal.ThrowExceptionForHR(hr);

            IAudioMeterInformation meter = meterObj as IAudioMeterInformation;
            if (meter == null)
                throw new InvalidCastException("Unable to cast to IAudioMeterInformation.");

            // Get and return the current peak (loudness) value.
            float peak;
            hr = meter.GetPeakValue(out peak);
            if (hr != 0) Marshal.ThrowExceptionForHR(hr);

            return peak;
        }
    }
}
