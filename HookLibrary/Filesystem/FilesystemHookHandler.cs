using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using EasyHook;
using HookLibrary.Filesystem.Host;
using HookLibrary.Filesystem.Host.NativeTypes;
using NLog;

namespace HookLibrary.Filesystem
{
    public static class FilesystemHookHandler
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
        /// <param name="eaBuffer">Pointer to extended attributes buffer.</param>
        /// <param name="eaLength">Length of the extended attributes buffer.</param>
        /// <returns></returns>
        public static NtStatus NtCreateFile(
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
            uint eaLength)
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            NtStatus status;
            try
            {
                if (helper != null)
                {
                    var originalPath = objectAttributes.GetNtPath();
                    var redirectedPath = helper.RedirectorInterface.RedirectPath(originalPath, null);
                    var redirectedObjectAttributes = new ObjectAttributes
                    {
                        Attributes = objectAttributes.Attributes,
                        ObjectName = new UnicodeString(redirectedPath),
                        SecurityDescriptor = objectAttributes.SecurityDescriptor,
                        SecurityQos = objectAttributes.SecurityQos,
                    };

                    redirectedObjectAttributes.Length = objectAttributes.Length - objectAttributes.ObjectName.Length +
                                                        redirectedObjectAttributes.ObjectName.Length;

                    status = NativeApi.NtCreateFile(out handle, access, ref redirectedObjectAttributes, ref ioStatusBlock,
                        allocationSize, fileAttributes, share, createDisposition, createOptions, eaBuffer, eaLength);
                }
                else
                {
                    status = NativeApi.NtCreateFile(out handle, access, ref objectAttributes, ref ioStatusBlock,
                        allocationSize, fileAttributes, share, createDisposition, createOptions, eaBuffer, eaLength);
                }
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtCreateFile] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"AccessMask:{{{access}}}, " +
                $"Disposition: {createDisposition}, " +
                $"CreateOptions: {createOptions}, " +
                $"Status: {status}"
                );

            return status;
        }

        public static NtStatus NtOpenFile(out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            ref IoStatusBlock ioStatusBlock,
            FileShare share,
            NtFileOptions openOptions)
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            // TODO: redirection.
            
            var status = NativeApi.NtOpenFile(out handle, access, ref objectAttributes, ref ioStatusBlock,
                share, openOptions);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtOpenFile] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"AccessMask:{{{access}}}, " +
                $"ShareOptions: {share}, " +
                $"OpenOptions: {openOptions}, " +
                $"Status: {status}"
                );

            return status;
        }

        public static NtStatus NtDeleteFile(ref ObjectAttributes objectAttributes // in
            )
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            // TODO: redirection.
            var status = NativeApi.NtDeleteFile(ref objectAttributes);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtDeleteFile] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"Status: {status}");

            return status;
        }

        public static NtStatus NtQueryAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            ref FileBasicInformation fileBasicInfo // out
            )
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            var status = NativeApi.NtQueryAttributesFile(ref objectAttributes, ref fileBasicInfo);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtQueryAttributesFile] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"Status: {status}, " +
                $"FileBasicInfo:{{{(status == NtStatus.ObjectNameNotFound ? null : fileBasicInfo.ToString())}}}.");

            return status;
        }

        public static NtStatus NtQueryFullAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            out IntPtr attributes // out
            )
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            // TODO: redirection.
            var status = NativeApi.NtQueryFullAttributesFile(ref objectAttributes, out attributes);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtQueryFullAttributesFile] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}," +
                $"Status: {status}");

            return status;
        }

        public static NtStatus NtOpenSymbolicLinkObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            // TODO: redirection.
            var status = NativeApi.NtOpenSymbolicLinkObject(out handle, access, ref objectAttributes);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtOpenSymbolicLinkObject] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"AccessMask: {access}, " +
                $"Status: {status}.");

            return status;
        }

        public static NtStatus NtOpenDirectoryObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // need to get this as early as possible
            HookEntryPoint helper;
            try
            {
                helper = (HookEntryPoint) HookRuntimeInfo.Callback;
            }
            catch
            {
                helper = null;
            }

            // TODO: redirection.
            var status = NativeApi.NtOpenDirectoryObject(out handle, access, ref objectAttributes);

            helper?.AddToLogQueue(LogLevel.Debug,
                "[NtOpenDirectoryObject] " +
                $"ObjectName: {objectAttributes.ObjectName.WithoutPrefix()}, " +
                $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                $"AccessMask: {access}, " +
                $"Status: {status}.");

            return status;
        }
    }
}