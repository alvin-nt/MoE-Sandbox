using System;
using System.IO;
using System.Runtime.InteropServices;
using HookLibrary.Filesystem.Host.NativeTypes;

namespace HookLibrary.Filesystem.Host
{
    public static partial class NativeApi
    {
        /// <summary>
        /// Represents all delegates for the native system calls that is going to be hooked.        
        /// </summary>
        public class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtCreateFile(
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
            public delegate NtStatus NtOpenFile(
                out IntPtr handle,
                AccessMask access,
                ref ObjectAttributes objectAttributes,
                out IoStatusBlock ioStatusBlock,
                FileShare share,
                NtFileOptions openOptions);

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtDeleteFile(
                ref ObjectAttributes objectAttributes // in
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtQueryAttributesFile(
                ref ObjectAttributes objectAttributes, // in
                ref FileBasicInformation fileBasicInfo // out
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtQueryFullAttributesFile(
                ref ObjectAttributes objectAttributes, // in
                out IntPtr attributes // out
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtOpenSymbolicLinkObject(
                out IntPtr handle, // out
                AccessMask access, // in
                ref ObjectAttributes objectAttributes // in
                );

            [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
            public delegate NtStatus NtOpenDirectoryObject(
                out IntPtr handle, // out
                AccessMask access, // in
                ref ObjectAttributes objectAttributes // in
                );
        }
    }
}
