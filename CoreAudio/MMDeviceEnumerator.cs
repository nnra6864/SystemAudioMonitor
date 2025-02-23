using System;
using System.Runtime.InteropServices;
using NnUtils.Modules.SystemAudioMonitor.Windows.Interfaces;

namespace NnUtils.Modules.SystemAudioMonitor.Windows
{

    /// <summary>
    /// MM Device Enumerator
    /// </summary>
    public class MMDeviceEnumerator : IDisposable
    {
        private IMMDeviceEnumerator _realEnumerator;

        /// <summary>
        /// Creates a new MM Device Enumerator
        /// </summary>
        public MMDeviceEnumerator()
        {
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
            _realEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
        }

        /// <summary>
        /// Enumerate Audio Endpoints
        /// </summary>
        /// <param name="dataFlow">Desired DataFlow</param>
        /// <param name="dwStateMask">State Mask</param>
        /// <returns>Device Collection</returns>
        public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState dwStateMask)
        {
            Marshal.ThrowExceptionForHR(_realEnumerator.EnumAudioEndpoints(dataFlow, dwStateMask, out var result));
            return new MMDeviceCollection(result);
        }

        /// <summary>
        /// Get Default Endpoint
        /// </summary>
        /// <param name="dataFlow">Data Flow</param>
        /// <param name="role">Role</param>
        /// <returns>Device</returns>
        public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
        {
            Marshal.ThrowExceptionForHR(((IMMDeviceEnumerator)_realEnumerator).GetDefaultAudioEndpoint(dataFlow, role, out var device));
            return new MMDevice(device);
        }

        /// <summary>
        /// Check to see if a default audio end point exists without needing an exception.
        /// </summary>
        /// <param name="dataFlow">Data Flow</param>
        /// <param name="role">Role</param>
        /// <returns>True if one exists, and false if one does not exist.</returns>
        public bool HasDefaultAudioEndpoint(DataFlow dataFlow, Role role)
        {
            const int E_NOTFOUND = unchecked((int)0x80070490);
            int hresult = ((IMMDeviceEnumerator)_realEnumerator).GetDefaultAudioEndpoint(dataFlow, role, out var device);
            if (hresult == 0x0)
            {
                Marshal.ReleaseComObject(device);
                return true;
            }
            if (hresult == E_NOTFOUND)
            {
                return false;
            }
            Marshal.ThrowExceptionForHR(hresult);
            return false;
        }

        /// <summary>
        /// Get device by ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <returns>Device</returns>
        public MMDevice GetDevice(string id)
        {
            Marshal.ThrowExceptionForHR(((IMMDeviceEnumerator)_realEnumerator).GetDevice(id, out var device));
            return new MMDevice(device);
        }

        /// <summary>
        /// Registers a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
        public int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _realEnumerator.RegisterEndpointNotificationCallback(client);
        }

        /// <summary>
        /// Unregisters a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        public int UnregisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _realEnumerator.UnregisterEndpointNotificationCallback(client);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called to dispose/finalize contained objects.
        /// </summary>
        /// <param name="disposing">True if disposing, false if called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_realEnumerator != null)
                {
                    // although GC would do this for us, we want it done now
                    Marshal.ReleaseComObject(_realEnumerator);
                    _realEnumerator = null;
                }
            }
        }
    }
}