using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HookTest
{
    public static partial class NativeApi
    {
        public static class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
            public delegate IntPtr CreateFileA(string filename,
                [MarshalAs(UnmanagedType.U4)] FileAccess access,
                [MarshalAs(UnmanagedType.U4)] FileShare share,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
                IntPtr templateFile);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            public delegate IntPtr CreateFileW(string filename,
                [MarshalAs(UnmanagedType.U4)] FileAccess access,
                [MarshalAs(UnmanagedType.U4)] FileShare share,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
                IntPtr templateFile);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
            public delegate bool CreateDirectoryA(string filename, IntPtr securityAttributes);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            public delegate bool CreateDirectoryW(string filename, IntPtr securityAttributes);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
            public delegate bool DeleteFileA(string filename);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            public delegate bool DeleteFileW(string filename);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
            public delegate IntPtr LoadLibraryExA(string filename, IntPtr reserved, uint flags);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            public delegate IntPtr LoadLibraryExW(string filename, IntPtr reserved, uint flags);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
            public delegate bool RemoveDirectoryA(string filename);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
            public delegate bool RemoveDirectoryW(string filename);
        }
    }
}