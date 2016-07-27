using System;
using System.Diagnostics.Contracts;
using System.IO;
using HookLibrary.Filesystem.Host;
using HookLibrary.Filesystem.Host.NativeTypes;
using NLog;

namespace HookLibrary.Filesystem
{
    public class FilesystemHook
    {
        protected readonly FilesystemRedirector RedirectorInterface;

        public FilesystemHook(FilesystemRedirector redirector)
        {
            //Contract.Requires<ArgumentNullException>(redirector != null, "Redirector cannot be null!");

            RedirectorInterface = redirector;
        }

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
        public NtStatus NtCreateFile(
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
            uint eaLength)
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtCreateFile] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                                             $"AccessMask: {access}, " +
                                             $"Disposition: {createDisposition}, " +
                                             $"CreateOptions: {createOptions}");

            var status = NativeApi.NtCreateFile(out handle, access, ref objectAttributes, out ioStatusBlock,
                ref allocationSize, fileAttributes, share, createDisposition, createOptions, eaBuffer, eaLength);
            RedirectorInterface.Logger.Debug($"[NtCreateFile] Status: {status}");

            return status;
        }

        public NtStatus NtOpenFile(out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            out IoStatusBlock ioStatusBlock,
            FileShare share,
            NtFileOptions openOptions)
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtOpenFile] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                                             $"AccessMask: {access}, " +
                                             $"ShareOptions: {share}, " +
                                             $"OpenOptions: {openOptions}");

            var status = NativeApi.NtOpenFile(out handle, access, ref objectAttributes, out ioStatusBlock,
                share, openOptions);
            RedirectorInterface.Logger.Debug($"[NtOpenFile] Status: {status}");

            return status;
        }

        public NtStatus NtDeleteFile(ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtDeleteFile] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtDeleteFile(ref objectAttributes);
            RedirectorInterface.Logger.Debug($"[NtDeleteFile] Status: {status}");

            return status;
        }

        public NtStatus NtQueryAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            ref FileBasicInformation fileBasicInfo // out
            )
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtQueryAttributesFile] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtQueryAttributesFile(ref objectAttributes, ref fileBasicInfo);
            RedirectorInterface.Logger.Debug($"[NtQueryAttributesFile] Status: {status}, " +
                                             $"FileBasicInfo:{{{fileBasicInfo}}}");

            return status;
        }

        public NtStatus NtQueryFullAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            out IntPtr attributes // out
            )
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtQueryFullAttributesFile] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtQueryFullAttributesFile(ref objectAttributes, out attributes);
            RedirectorInterface.Logger.Debug($"[NtQueryAttributesFile] Status: {status}");

            return status;
        }

        public NtStatus NtOpenSymbolicLinkObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtOpenSymbolicLinkObject] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                                             $"AccessMask: {access}");

            var status = NativeApi.NtOpenSymbolicLinkObject(out handle, access, ref objectAttributes);
            RedirectorInterface.Logger.Debug($"[NtOpenSymbolicLinkObject] Status: {status}");

            return status;
        }

        public NtStatus NtOpenDirectoryObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            RedirectorInterface.Logger.Debug("[NtOpenDirectoryObject] " +
                                             $"ObjectName: {objectAttributes.ObjectName}, " +
                                             $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                             $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                                             $"AccessMask: {access}");

            var status = NativeApi.NtOpenDirectoryObject(out handle, access, ref objectAttributes);
            RedirectorInterface.Logger.Debug($"[NtOpenDirectoryObject] Status: {status}");

            return status;
        }
    }
}