using System;
using System.IO;
using System.Runtime.InteropServices;
using HookLibrary.Filesystem.Host.NativeTypes;

namespace HookLibrary.Filesystem.Host
{
    /// <summary>
    /// Contains calls for Windows native API.
    /// </summary>
    /// <remarks>Not all functions are hooked.</remarks>
    public static partial class NativeApi
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="access"></param>
        /// <param name="objectAttributes"></param>
        /// <param name="ioStatusBlock"></param>
        /// <param name="allocationSize"></param>
        /// <param name="fileAttributes"></param>
        /// <param name="share"></param>
        /// <param name="createDisposition"></param>
        /// <param name="createOptions"></param>
        /// <param name="eaBuffer"></param>
        /// <param name="eaLength"></param>
        /// <returns></returns>
        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtCreateFile(
            out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            ref IoStatusBlock ioStatusBlock,
            long allocationSize,
            uint fileAttributes,
            FileShare share,
            NtFileCreateDisposition createDisposition,
            NtFileOptions createOptions,
            IntPtr eaBuffer,
            uint eaLength
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtOpenFile(
            out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            ref IoStatusBlock ioStatusBlock,
            FileShare share,
            NtFileOptions openOptions
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtDeleteFile(
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtQueryAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            ref FileBasicInformation fileBasicInfo // out
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtQueryFullAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            out IntPtr attributes // out
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        public static extern NtStatus NtOpenDirectoryObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]        
        public static extern NtStatus NtOpenSymbolicLinkObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]        
        public static extern NtStatus NtQueryInformationFile(
            IntPtr handle,
            out IoStatusBlock ioStatusBlock,
            out IntPtr fileInformation,
            uint length,
            FileInformationClass informationClass
            );
    }
}