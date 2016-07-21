﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Policy;
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
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtCreateFile(
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
            uint eaLength
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtOpenFile(
            out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            out IoStatusBlock ioStatusBlock,
            FileShare share,
            NtFileOptions openOptions);

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtDeleteFile(
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtQueryAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            ref FileBasicInformation fileBasicInfo // out
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtQueryFullAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            out IntPtr attributes // out
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtOpenDirectoryObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtOpenSymbolicLinkObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            );

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern NtStatusCode NtQueryInformationFile(
            IntPtr handle,
            out IoStatusBlock ioStatusBlock,
            out IntPtr fileInformation,
            uint length,
            FileInformationClass informationClass
            );
    }
}