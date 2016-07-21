using System;
using System.IO;
using HookLibrary.Filesystem.Host;
using HookLibrary.Filesystem.Host.NativeTypes;
using NLog;

namespace HookLibrary.Filesystem
{
    public class FilesystemHook : MarshalByRefObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
        public NtStatusCode NtCreateFile(
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
            Logger.Log(LogLevel.Debug, "[NtCreateFile] " +
                                       $"ObjectName: {objectAttributes.ObjectName}, " +
                                       $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                                       $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                                       $"AccessMask: {access}, " +
                                       $"Disposition: {createDisposition}, " +
                                       $"CreateOptions: {createOptions}");

            var status = NativeApi.NtCreateFile(out handle, access, ref objectAttributes, out ioStatusBlock,
                ref allocationSize, fileAttributes, share, createDisposition, createOptions, eaBuffer, eaLength);
            Logger.Log(LogLevel.Debug, $"[NtCreateFile] Status: {status}");

            return status;
        }

        public NtStatusCode NtOpenFile(out IntPtr handle,
            AccessMask access,
            ref ObjectAttributes objectAttributes,
            out IoStatusBlock ioStatusBlock,
            FileShare share,
            NtFileOptions openOptions)
        {
            // TODO: redirection.
            Logger.Debug("[NtOpenFile] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                         $"AccessMask: {access}, " +
                         $"ShareOptions: {share}, " +
                         $"OpenOptions: {openOptions}");

            var status = NativeApi.NtOpenFile(out handle, access, ref objectAttributes, out ioStatusBlock,
                share, openOptions);
            Logger.Debug($"[NtOpenFile] Status: {status}");

            return status;
        }

        public NtStatusCode NtDeleteFile(ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            Logger.Debug("[NtDeleteFile] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtDeleteFile(ref objectAttributes);
            Logger.Debug($"[NtDeleteFile] Status: {status}");

            return status;
        }

        public NtStatusCode NtQueryAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            ref FileBasicInformation fileBasicInfo // out
            )
        {
            // TODO: redirection.
            Logger.Debug("[NtQueryAttributesFile] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtQueryAttributesFile(ref objectAttributes, ref fileBasicInfo);
            Logger.Debug($"[NtQueryAttributesFile] Status: {status}, " +
                         $"FileBasicInfo:{{{fileBasicInfo}}}");

            return status;
        }

        public NtStatusCode NtQueryFullAttributesFile(
            ref ObjectAttributes objectAttributes, // in
            out IntPtr attributes // out
            )
        {
            // TODO: redirection.
            Logger.Debug("[NtQueryFullAttributesFile] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}");

            var status = NativeApi.NtQueryFullAttributesFile(ref objectAttributes, out attributes);
            Logger.Debug($"[NtQueryAttributesFile] Status: {status}");

            return status;
        }

        public NtStatusCode NtOpenSymbolicLinkObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            Logger.Debug("[NtOpenSymbolicLinkObject] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                         $"AccessMask: {access}");

            var status = NativeApi.NtOpenSymbolicLinkObject(out handle, access, ref objectAttributes);
            Logger.Debug($"[NtOpenSymbolicLinkObject] Status: {status}");

            return status;
        }

        public NtStatusCode NtOpenDirectoryObject(
            out IntPtr handle, // out
            AccessMask access, // in
            ref ObjectAttributes objectAttributes // in
            )
        {
            // TODO: redirection.
            Logger.Debug("[NtOpenDirectoryObject] " +
                         $"ObjectName: {objectAttributes.ObjectName}, " +
                         $"FullNtPath: {objectAttributes.GetNtPath()}, " +
                         $"FullDosPath: {objectAttributes.GetDosPath()}, " +
                         $"AccessMask: {access}");

            var status = NativeApi.NtOpenDirectoryObject(out handle, access, ref objectAttributes);
            Logger.Debug($"[NtOpenDirectoryObject] Status: {status}");

            return status;
        }
    }
}