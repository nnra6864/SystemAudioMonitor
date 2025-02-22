using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class WindowsAudioMonitor : AudioMonitor
    {
        #region Native Methods and Types
        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        private class MMDeviceEnumerator { }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            int NotImpl1();
            int NotImpl2();
            [PreserveSig]
            int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppEndpoint);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        }

        [Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioMeterInformation
        {
            float GetPeakValue();
        }

        private const int CLSCTX_ALL = 0x1 | 0x2 | 0x4 | 0x10;
        private const int DEVICE_STATE_ACTIVE = 0x1;
        private const int ROLE_MULTIMEDIA = 1;
        private const int DATAFLOW_RENDER = 1; // output devices
        #endregion

        private IMMDeviceEnumerator _deviceEnumerator;
        private IMMDevice _device;
        private IAudioMeterInformation _meterInfo;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        public readonly int UpdateInterval;

        public WindowsAudioMonitor(int updateInterval = 50)
        {
            _deviceEnumerator = new MMDeviceEnumerator() as IMMDeviceEnumerator;
            UpdateInterval = updateInterval;
        }

        public override async Task Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _cancellationTokenSource = new();

            // Get the default audio output device
            _deviceEnumerator.GetDefaultAudioEndpoint(DATAFLOW_RENDER, ROLE_MULTIMEDIA, out _device);

            // Get the meter information interface
            var audioMeterGuid = typeof(IAudioMeterInformation).GUID;
            _device.Activate(ref audioMeterGuid, CLSCTX_ALL, IntPtr.Zero, out var meterObj);
            _meterInfo = (IAudioMeterInformation)meterObj;

            await Task.Run(() => MonitorAudio(_cancellationTokenSource.Token, UpdateInterval));
        }

        private void MonitorAudio(CancellationToken token, int updateIntervalMs)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Loudness = _meterInfo.GetPeakValue();
                    Thread.Sleep(updateIntervalMs);
                }
            }
            finally
            {
                _isRunning = false;
            }
        }

        private void StopMonitoring()
        {
            if (!_isRunning) return;
            _cancellationTokenSource?.Cancel();
            _isRunning = false;
        }

        public override void Dispose()
        {
            StopMonitoring();
            if (_meterInfo != null) Marshal.ReleaseComObject(_meterInfo);
            if (_device != null) Marshal.ReleaseComObject(_device);
            if (_deviceEnumerator != null) Marshal.ReleaseComObject(_deviceEnumerator);
        }
    }
}