using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NnUtils.Modules.SystemAudioMonitor.CoreAudio
{
    /// <summary>
    /// from Propidl.h.
    /// http://msdn.microsoft.com/en-us/library/aa380072(VS.85).aspx
    /// contains a union so we have to do an explicit layout
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PropVariant
    {
        /// <summary>
        /// Value type tag.
        /// </summary>
        [FieldOffset(0)] public short vt;
        
        /// <summary>
        /// Reserved1.
        /// </summary>
        [FieldOffset(2)] public short wReserved1;
        
        /// <summary>
        /// Reserved2.
        /// </summary>
        [FieldOffset(4)] public short wReserved2;
        
        /// <summary>
        /// Reserved3.
        /// </summary>
        [FieldOffset(6)] public short wReserved3;
        
        /// <summary>
        /// cVal.
        /// </summary>
        [FieldOffset(8)] public sbyte cVal;
        
        /// <summary>
        /// bVal.
        /// </summary>
        [FieldOffset(8)] public byte bVal;
        
        /// <summary>
        /// iVal.
        /// </summary>
        [FieldOffset(8)] public short iVal;
        
        /// <summary>
        /// uiVal.
        /// </summary>
        [FieldOffset(8)] public ushort uiVal;
        
        /// <summary>
        /// lVal.
        /// </summary>
        [FieldOffset(8)] public int lVal;
        
        /// <summary>
        /// ulVal.
        /// </summary>
        [FieldOffset(8)] public uint ulVal;
        
        /// <summary>
        /// intVal.
        /// </summary>
        [FieldOffset(8)] public int intVal;
        
        /// <summary>
        /// uintVal.
        /// </summary>
        [FieldOffset(8)] public uint uintVal;
        
        /// <summary>
        /// hVal.
        /// </summary>
        [FieldOffset(8)] public long hVal;
        
        /// <summary>
        /// uhVal.
        /// </summary>
        [FieldOffset(8)] public long uhVal;
        
        /// <summary>
        /// fltVal.
        /// </summary>
        [FieldOffset(8)] public float fltVal;
        
        /// <summary>
        /// dblVal.
        /// </summary>
        [FieldOffset(8)] public double dblVal;
        
        //VARIANT_BOOL boolVal;
        /// <summary>
        /// boolVal.
        /// </summary>
        [FieldOffset(8)] public short boolVal;
        
        /// <summary>
        /// scode.
        /// </summary>
        [FieldOffset(8)] public int scode;

        //CY cyVal;
        //[FieldOffset(8)] private DateTime date; - can cause issues with invalid value
        /// <summary>
        /// Date time.
        /// </summary>
        [FieldOffset(8)] public System.Runtime.InteropServices.ComTypes.FILETIME filetime;
        
        //CLSID* puuid;
        //CLIPDATA* pclipdata;
        //BSTR bstrVal;
        //BSTRBLOB bstrblobVal;
        /// <summary>
        /// Binary large object.
        /// </summary>
        [FieldOffset(8)] public Blob blobVal;
        
        //LPSTR pszVal;
        /// <summary>
        /// Pointer value.
        /// </summary>
        [FieldOffset(8)] public IntPtr pointerValue; //LPWSTR

        private static class VT
        {
            public const short I1 = 16; // VT_I1
            public const short I2 = 2; // VT_I2
            public const short I4 = 3; // VT_I4
            public const short I8 = 20; // VT_I8
            public const short INT = 22; // VT_INT
            public const short UI4 = 19; // VT_UI4
            public const short UI8 = 21; // VT_UI8
            public const short LPWSTR = 31; // VT_LPWSTR
            public const short BLOB = 65; // VT_BLOB
            public const short VECTOR = 0x1000; // VT_VECTOR
            public const short UI1 = 17; // VT_UI1
            public const short CLSID = 72; // VT_CLSID
            public const short BOOL = 11; // VT_BOOL
            public const short FILETIME = 64; // VT_FILETIME
            public const short EMPTY = 0; // VT_EMPTY
        }

        /// <summary>
        /// Creates a new PropVariant containing a long value
        /// </summary>
        public static PropVariant FromLong(long value) => new() { vt = VT.I8, hVal = value };

        /// <summary>
        /// Helper method to gets blob data
        /// </summary>
        private byte[] GetBlob()
        {
            var blob = new byte[blobVal.Length];
            Marshal.Copy(blobVal.Data, blob, 0, blob.Length);
            return blob;
        }

        /// <summary>
        /// Interprets a blob as an array of structs
        /// </summary>
        public T[] GetBlobAsArrayOf<T>()
        {
            var blobByteLength = blobVal.Length;
            var singleInstance = (T) Activator.CreateInstance(typeof (T));
            var structSize = Marshal.SizeOf(singleInstance);
            
            if (blobByteLength%structSize != 0)
                throw new InvalidDataException($"Blob size {blobByteLength} not a multiple of struct size {structSize}");
            
            var items = blobByteLength/structSize;
            var array = new T[items];
            for (int n = 0; n < items; n++)
            {
                array[n] = (T) Activator.CreateInstance(typeof (T));
                Marshal.PtrToStructure(new((long) blobVal.Data + n*structSize), array[n]);
            }
            
            return array;
        }

        /// <summary>
        /// Property value
        /// </summary>
        public object Value =>
            vt switch
            {
                VT.I1 => bVal,
                VT.I2 => iVal,
                VT.I4 => lVal,
                VT.I8 => hVal,
                VT.INT => iVal,
                VT.UI4 => ulVal,
                VT.UI8 => uhVal,
                VT.LPWSTR => Marshal.PtrToStringUni(pointerValue),
                VT.BLOB or (VT.VECTOR | VT.UI1) => GetBlob(),
                VT.CLSID => Marshal.PtrToStructure<Guid>(pointerValue),
                VT.BOOL => boolVal switch
                {
                    -1 => true,
                    0 => false,
                    _ => throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0")
                },
                VT.FILETIME => DateTime.FromFileTime((((long)filetime.dwHighDateTime) << 32) + filetime.dwLowDateTime),
                VT.EMPTY => null,
                _ => throw new NotImplementedException("PropVariant " + vt)
            };

        /// <summary>
        /// Clears with a known pointer
        /// </summary>
        public static void Clear(IntPtr ptr) => PropVariantNative.PropVariantClear(ptr);
    }
}