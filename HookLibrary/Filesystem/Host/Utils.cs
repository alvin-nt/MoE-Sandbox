using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using HookLibrary.Filesystem.Host.NativeTypes;
using Microsoft.Win32;

namespace HookLibrary.Filesystem.Host
{
    internal class Utils
    {
        /// <summary>
        /// Constant for invalid handle value (-1)
        /// </summary>
        /// <remarks>
        /// Source: http://permalink.gmane.org/gmane.comp.java.jna.user/4538
        /// </remarks>
        private static readonly IntPtr InvalidHandleValue = new IntPtr(0xFFFFFFFF);

        /// <summary>
        /// Queries information from a NtHandle
        /// </summary>
        /// <param name="ntHandle"></param>
        /// <param name="informationClass"></param>
        /// <param name="infoPtr"></param>
        /// <param name="infoLength"></param>
        /// <param name="returnLength"></param>
        /// <returns></returns>
        /// <remarks>
        /// As told in http://stackoverflow.com/a/18792477/2935556,
        /// we cannot trust the return code of this function.
        /// Therefore, its return code will be ignored.
        /// </remarks>
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true
            )]
        [ResourceExposure(ResourceScope.Machine)]
        private static extern NtStatusCode NtQueryObject(IntPtr ntHandle,
            ObjectInformationClass informationClass,
            IntPtr infoPtr,
            uint infoLength,
            ref uint returnLength);

        /// <summary>
        /// UNICODE_STRING struct, used for
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
        private struct UnsafeUnicodeString
        {
            internal ushort Length;
            internal ushort MaximumLength;
            internal unsafe char* buffer;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
        private struct ObjectNameInformation
        {
            internal UnsafeUnicodeString Name;
            internal unsafe char* NameBuffer;
        }

        /// <summary>
        /// Get the relevant path from a NtHandle.
        /// </summary>
        /// <param name="ntHandle">the Nt-Handle, which is created by handle-generating Nt-xxx systemcalls</param>
        /// <param name="ntPath">Resulting path</param>
        /// <returns></returns>
        /// <remarks>
        /// Currently, this function is written using P/Invoke-style.
        /// TODO: refactor it, so that this function throws exception.
        /// </remarks>
        public static NtStatusCode GetNtPathFromHandle(IntPtr ntHandle, out string ntPath)
        {
            ntPath = "";
            if (ntHandle == IntPtr.Zero || ntHandle == InvalidHandleValue)
            {
                return NtStatusCode.InvalidHandle;
            }

            // checks whether it is a console handle
            if ((ntHandle.ToInt32() & 0x10000003) == 0x3)
            {
                ntPath = $@"\Device\Console{ntHandle.ToInt32():X4}";
                return NtStatusCode.Success;
            }

            unsafe // this part of code uses pointer manipulation, so this needs to be marked as unsafe.
            {
                const int bufLen = 2000;
                var buffer = Marshal.AllocHGlobal(bufLen);
                uint length = 0;

                var info = &((ObjectNameInformation*) buffer.ToPointer())->Name;
                info->buffer = null;
                info->Length = 0;

                NtQueryObject(ntHandle, ObjectInformationClass.ObjectNameInformation, buffer, bufLen, ref length);

                if (info->buffer == null)
                {
                    return NtStatusCode.ObjectNameNotFound;
                }

                // set endchar to null
                info->buffer[info->Length/UnicodeEncoding.CharSize] = (char) 0;
                ntPath = Marshal.PtrToStringUni(new IntPtr(info->buffer), info->Length);
                Marshal.FreeHGlobal(buffer);
            }

            return NtStatusCode.Success;
        }

        /// <summary>
        /// Retrieves information about DOS device names.
        /// </summary>
        /// <param name="deviceName">DOS device name, without trailing backslash (e.g. 'C:', not 'C:\')</param>
        /// <param name="buffer">
        /// A pointer to a buffer that will receive the result of the query. The function fills this buffer with one or more null-terminated strings. The final null-terminated string is followed by an additional NULL.
        /// If deviceName is non-NULL, the function retrieves information about the particular MS-DOS device specified by deviceName. The first null-terminated string stored into the buffer is the current mapping for the device. The other null-terminated strings represent undeleted prior mappings for the device.
        /// If deviceName is NULL, the function retrieves a list of all existing MS-DOS device names. Each null-terminated string stored into the buffer is the name of an existing MS-DOS device, for example, \Device\HarddiskVolume1 or \Device\Floppy0.
        /// </param>
        /// <param name="maxUnicodeCharLen">The maximum number of Unicode characters that can be stored into the buffer pointed to by <see cref="lpTargetPath"/>.</param>
        /// <returns>
        /// If the function succeeds, the return value is the number of TCHARs stored into the buffer pointed to by buffer.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// If the buffer is too small, the function fails and the last error code is <see cref="WinStatusCode.ERROR_INSUFFICIENT_BUFFER"/>.
        /// </returns>

        [DllImport("kernel32.dll")]
        protected static extern WinStatusCode QueryDosDevice(
            [MarshalAs(UnmanagedType.LPStr)] string deviceName, 
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, 
            uint maxUnicodeCharLen);

        /// <summary>
        /// Transforms NtPath to a corresponding DOS path.
        /// </summary>
        /// <returns>
        /// "\Device\HarddiskVolume3"                                -> "E:"
        /// "\Device\HarddiskVolume3\Temp"                           -> "E:\Temp"
        /// "\Device\HarddiskVolume3\Temp\transparent.jpeg"          -> "E:\Temp\transparent.jpeg"
        /// "\Device\Harddisk1\DP(1)0-0+6\foto.jpg"                  -> "I:\foto.jpg"
        /// "\Device\TrueCryptVolumeP\Data\Passwords.txt"            -> "P:\Data\Passwords.txt"
        /// "\Device\Floppy0\Autoexec.bat"                           -> "A:\Autoexec.bat"
        /// "\Device\CdRom1\VIDEO_TS\VTS_01_0.VOB"                   -> "H:\VIDEO_TS\VTS_01_0.VOB"
        /// "\Device\Serial1"                                        -> "COM1"
        /// "\Device\USBSER000"                                      -> "COM4"
        /// "\Device\Mup\ComputerName\C$\Boot.ini"                   -> "\\ComputerName\C$\Boot.ini"
        /// "\Device\LanmanRedirector\ComputerName\C$\Boot.ini"      -> "\\ComputerName\C$\Boot.ini"
        /// "\Device\LanmanRedirector\ComputerName\Shares\Dance.m3u" -> "\\ComputerName\Shares\Dance.m3u"
        /// returns <see cref="WinStatusCode.ERROR_BAD_PATHNAME"/> for any other device type
        /// </returns>
        public static WinStatusCode GetDosPathFromNtPath(string ntPath, out string dosPath)
        {
            dosPath = "";

            // check for USB or serial paths
            if (ntPath.StartsWith(@"\Device\Serial", StringComparison.CurrentCultureIgnoreCase) ||
                ntPath.StartsWith(@"\Device\UsbSer", StringComparison.CurrentCultureIgnoreCase) ||
                ntPath.StartsWith(@"\Device\ssudmdm", StringComparison.CurrentCultureIgnoreCase) /* SAMSUNG USB devices */)
            {
                // query the COM port form the registry
                var regValue = Registry.GetValue(RegistryHive.LocalMachine + @"\Hardware\DeviceMap\SerialComm",
                    ntPath,
                    null);
                if (regValue == null)
                {
                    return WinStatusCode.ERROR_UNKNOWN_PORT;
                }

                dosPath = (string) regValue;
                return WinStatusCode.Success;
            }

            // Valid since Windows XP
            // Used for querying network drives
            if (ntPath.StartsWith(@"\Device\LanmanRedirector\"))
            {
                // double string for network paths
                dosPath = $@"\\{ntPath.Substring(25)}";
                return WinStatusCode.Success;
            }

            // For Windows 7 - Multiple UNC Provider
            // For more information: https://msdn.microsoft.com/en-us/windows/hardware/drivers/ifs/support-for-unc-naming-and-mup
            if (ntPath.StartsWith(@"\Device\Mup"))
            {
                // double string for network paths
                dosPath = $@"\\{ntPath.Substring(12)}";
                return WinStatusCode.Success;
            }

            // now for the drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                // just take the first 2 characters (e.g. H:, A:)
                var dosDriveName = drive.Name.Take(2).ToString();

                // arbritary buffer size
                var volumeBuffer = new StringBuilder(500);
                var queryDosDeviceRetCode = QueryDosDevice(dosDriveName, volumeBuffer,
                    (uint) volumeBuffer.Capacity / UnicodeEncoding.CharSize);

                switch (queryDosDeviceRetCode)
                {
                    case 0: // no data returned; something wrong happened.
                        return (WinStatusCode)Marshal.GetLastWin32Error();
                    case WinStatusCode.BufferTooSmall:
                        throw new Exception("Buffer size to small for QueryDosDevice()!");
                    default:
                        var volumeLen = volumeBuffer.Length/UnicodeEncoding.CharSize;
                        if (volumeBuffer.ToString().StartsWith(ntPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            dosPath = dosDriveName + ntPath.Substring(volumeLen);
                            return WinStatusCode.Success;
                        }
                        break;
                }
            }

            // other paths: treat as bad path name.
            // TODO: verify the existence of other paths
            return WinStatusCode.ERROR_BAD_PATHNAME;
        }
    }
}