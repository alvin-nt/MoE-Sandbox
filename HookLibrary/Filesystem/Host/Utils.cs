using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using HookLibrary.Filesystem.Host.NativeTypes;
using Microsoft.Win32;

namespace HookLibrary.Filesystem.Host
{
    public static class Utils
    {
        /// <summary>
        /// Constant for invalid handle value (-1)
        /// </summary>
        /// <remarks>
        /// Source: http://permalink.gmane.org/gmane.comp.java.jna.user/4538
        /// </remarks>
        private static readonly IntPtr InvalidHandleValue = new IntPtr(0xFFFFFFFF);

        /// <summary>
        /// Stores mapping
        /// </summary>
        /// <remarks>
        /// Item1 contains the Dos drive name
        /// Item2 contains the corresponding Nt path.
        /// </remarks>
        private static readonly List<Tuple<string, string>> NtDosDrives;

        /// <summary>
        /// Queries information from a handle
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
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        private static extern NtStatus NtQueryObject(IntPtr ntHandle,
            ObjectInformationClass informationClass,
            IntPtr infoPtr,
            int infoLength,
            ref int returnLength);

        /// <summary>
        /// Retrieves information about DOS device names.
        /// </summary>
        /// <param name="deviceName">DOS device name, without trailing backslash (e.g. 'C:', not 'C:\')</param>
        /// <param name="buffer">
        /// A pointer to a buffer that will receive the result of the query. The function fills this buffer with one or more null-terminated strings. The final null-terminated string is followed by an additional NULL.
        /// If deviceName is non-NULL, the function retrieves information about the particular MS-DOS device specified by deviceName.
        /// The first null-terminated string stored into the buffer is the current mapping for the device.
        /// The other null-terminated strings represent undeleted prior mappings for the device.
        /// If deviceName is NULL, the function retrieves a list of all existing MS-DOS device names.
        /// Each null-terminated string stored into the buffer is the name of an existing MS-DOS device, for example, \Device\HarddiskVolume1 or \Device\Floppy0.
        /// </param>
        /// <param name="maxUnicodeCharLen">The maximum number of Unicode characters that can be stored into the buffer pointed to by <see cref="lpTargetPath"/>.</param>
        /// <returns>
        /// If the function succeeds, the return value is the number of TCHARs stored into the buffer pointed to by buffer.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// If the buffer is too small, the function fails and the last error code is <see cref="WinStatusCode.ERROR_INSUFFICIENT_BUFFER"/>.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern WinStatusCode QueryDosDevice(
            [MarshalAs(UnmanagedType.LPStr)] string deviceName,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            uint maxUnicodeCharLen);

        /// <summary>
        /// Constructor. Initializes the mapping from Nt-to-Dos path.
        /// </summary>
        static Utils()
        {
            // TODO: temporarily disable hooking when this function is executed.

            var drives = DriveInfo.GetDrives();

            NtDosDrives = new List<Tuple<string, string>>(drives.Length);

            // load the DOS to Nt path mapping
            foreach (var drive in drives)
            {
                // just take the first 2 characters (e.g. H:, A:)
                var dosDriveName = drive.Name.Substring(0, 2);

                // arbritary buffer size
                var volumeBuffer = new StringBuilder(500);
                var queryDosDeviceRetCode = QueryDosDevice(dosDriveName, volumeBuffer,
                    (uint)volumeBuffer.Capacity);

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (queryDosDeviceRetCode)
                {
                    case 0: // no data returned; something wrong happened.
                        break; // just ignore?
                    case WinStatusCode.BufferTooSmall:
                        throw new Exception("Buffer size to small for QueryDosDevice()!");
                    default:
                        var ntVolumeString = volumeBuffer.ToString();
                        NtDosDrives.Add(new Tuple<string, string>(dosDriveName, ntVolumeString));
                        break;
                }
            }

        }

        /// <summary>
        /// Contains name data of a handle, resulting from <see cref="NtQueryObject"/> call.
        /// </summary>
        /// <remarks>
        /// Source: http://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FType%20independed%2FOBJECT_NAME_INFORMATION.html
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
        private struct ObjectNameInformation
        {
            internal UnicodeString Name;
            // the original struct's signature actually contains two elements:
            //     typedef struct _OBJECT_NAME_INFORMATION
            //     {
            //        UNICODE_STRING Name;
            //        WCHAR NameBuffer[0];
            //     }
            //     OBJECT_NAME_INFORMATION, *POBJECT_NAME_INFORMATION;
            // however, since the second element is a zero-sized array, and is not used in our case, we can safely ignore it.
            // Zero-sized array in the end of a C-struct means that the struct has a variable size.
        }

        /// <summary>
        /// Get the relevant path from a pointer to an Nt object.
        /// </summary>
        /// <param name="handle">the Nt-Handle, which is created by handle-generating Nt-xxx systemcalls</param>
        /// <param name="ntPath">Resulting path</param>
        /// <returns>
        /// "\Device\HarddiskVolume3"                                (Harddisk Drive)
        /// "\Device\HarddiskVolume3\Temp"                           (Harddisk Directory)
        /// "\Device\HarddiskVolume3\Temp\transparent.jpeg"          (Harddisk File)
        /// "\Device\Harddisk1\DP(1)0-0+6\foto.jpg"                  (USB stick)
        /// "\Device\TrueCryptVolumeP\Data\Passwords.txt"            (Truecrypt Volume)
        /// "\Device\Floppy0\Autoexec.bat"                           (Floppy disk)
        /// "\Device\CdRom1\VIDEO_TS\VTS_01_0.VOB"                   (DVD drive)
        /// "\Device\Serial1"                                        (real COM port)
        /// "\Device\USBSER000"                                      (virtual COM port)
        /// "\Device\Mup\ComputerName\C$\Boot.ini"                   (network drive share,  Windows 7)
        /// "\Device\LanmanRedirector\ComputerName\C$\Boot.ini"      (network drive share,  Windows XP)
        /// "\Device\LanmanRedirector\ComputerName\Shares\Dance.m3u" (network folder share, Windows XP)
        /// "\Device\Afd"                                            (internet socket)
        /// "\Device\Console000F"                                    (unique name for any Console handle)
        /// "\Device\NamedPipe\Pipename"                             (named pipe)
        /// "\BaseNamedObjects\Objectname"                           (named mutex, named event, named semaphore)
        /// "\REGISTRY\MACHINE\SOFTWARE\Classes\.txt"                (HKEY_CLASSES_ROOT\.txt)
        /// </returns>
        /// <remarks>
        /// Currently, this function is written using P/Invoke-style.
        /// Also, this cannot resolve all network links properly (see http://stackoverflow.com/questions/65170/how-to-get-name-associated-with-open-handle/5286888#comment43347121_18792477)
        /// TODO: refactor it, so that this function throws exception.
        /// </remarks>
        public static NtStatus GetNtPathFromHandle(IntPtr handle, out string ntPath)
        {
            ntPath = "";
            if (handle == IntPtr.Zero || handle == InvalidHandleValue)
            {
                return NtStatus.InvalidHandle;
            }

            // checks whether it is a console handle
            if ((handle.ToInt32() & 0x10000003) == 0x3)
            {
                ntPath = $@"\Device\Console{handle.ToInt32():X4}";
                return NtStatus.Success;
            }

            const int bufLen = 2000;
            var safeBuffer = Marshal.AllocHGlobal(bufLen);
            var safeLength = 0;

            // just ignore any error from this.
            NtQueryObject(handle, ObjectInformationClass.ObjectNameInformation, safeBuffer, bufLen, ref safeLength);

            var myStruct = (ObjectNameInformation) Marshal.PtrToStructure(safeBuffer, typeof(ObjectNameInformation));
            ntPath = myStruct.Name.ToString();

            Marshal.FreeHGlobal(safeBuffer);

            if (ntPath == "")
            {
                return (NtStatus) Marshal.GetLastWin32Error();
            }

            return NtStatus.Success;
        }

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
        /// returns <see cref="WinStatusCode.BadPathname"/> for any other device type
        /// </returns>
        public static WinStatusCode GetDosPathFromNtPath(string ntPath, out string dosPath)
        {
            dosPath = "";

            // check for USB or serial paths
            if (ntPath.StartsWith(@"\Device\Serial", StringComparison.CurrentCultureIgnoreCase) ||
                ntPath.StartsWith(@"\Device\UsbSer", StringComparison.CurrentCultureIgnoreCase) ||
                ntPath.StartsWith(@"\Device\ssudmdm", StringComparison.CurrentCultureIgnoreCase)
                /* SAMSUNG USB devices */)
            {
                // query the COM port form the registry
                const string regKey = @"HKEY_LOCAL_MACHINE\Hardware\DeviceMap\SerialComm";
                var regValue = Registry.GetValue(regKey, ntPath, null);
                if (regValue == null)
                {
                    return WinStatusCode.UnknownPort;
                }

                dosPath = (string) regValue;
                return WinStatusCode.Success;
            }

            // Valid since Windows XP
            // Used for querying network drives
            if (ntPath.StartsWith(@"\Device\LanmanRedirector\", StringComparison.CurrentCultureIgnoreCase))
            {
                // double string for network paths
                dosPath = $@"\\{ntPath.Substring(25)}";
                return WinStatusCode.Success;
            }

            // For Windows 7 - Multiple UNC Provider
            // For more information: https://msdn.microsoft.com/en-us/windows/hardware/drivers/ifs/support-for-unc-naming-and-mup
            if (ntPath.StartsWith(@"\Device\Mup", StringComparison.CurrentCultureIgnoreCase))
            {
                // double string for network paths
                dosPath = $@"\\{ntPath.Substring(12)}";
                return WinStatusCode.Success;
            }

            // now for the drives
            // TODO: cache the devices' names on the first run, for faster performance.
            foreach (var pathTuple in NtDosDrives)
            {
                var dosDrive = pathTuple.Item1;
                var ntPathForDosDrive = pathTuple.Item2;

                if (!ntPath.StartsWith(ntPathForDosDrive, StringComparison.CurrentCultureIgnoreCase)) continue;

                dosPath = dosDrive + ntPath.Substring(ntPathForDosDrive.Length);
                return WinStatusCode.Success;
            }

            // other paths: treat as bad path name.
            // TODO: verify the existence of other paths
            return WinStatusCode.BadPathname;
        }
    }
}