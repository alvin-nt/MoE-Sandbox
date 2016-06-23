using System;
using System.IO;
using System.Runtime.InteropServices;
using MoE_Sandbox.Virtualization.Filesystem.Host.NativeTypes;

namespace MoE_Sandbox.Virtualization.Filesystem.Host
{
    public static partial class NativeApi
    {
        /// <summary>
        /// Represents all delegates for the native system calls that is going to be hooked.        
        /// </summary>
        public static class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatusCode NtCreateFile(
                out IntPtr handle,
                AccessMask access,
                ref ObjectAttributes objectAttributes,
                out IoStatusBlock ioStatusBlock,
                ref long allocationSize,
                uint fileAttributes,
                FileShare share,
                NtFileCreateDisposition createDisposition,
                NtFileOptions createOptions,
                IntPtr eaBuffer,
                uint eaLength);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatusCode NtOpenFile(
                out IntPtr handle,
                AccessMask access,
                ref ObjectAttributes objectAttributes,
                out IoStatusBlock ioStatusBlock,
                FileShare share,
                NtFileOptions openOptions);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatusCode NtDeleteFile(
                ref ObjectAttributes objectAttributes // in
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatusCode NtQueryAttributesFile(
                ref ObjectAttributes objectAttributes, // in
                ref FileBasicInformation fileBasicInfo // out
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatusCode NtQueryFullAttributesFile(
                ref ObjectAttributes objectAttributes, // in
                out IntPtr attributes // out
                );
        }
    }
}
