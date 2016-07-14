using System;
using System.IO;
using MoE_Sandbox.Virtualization.Filesystem.Host;
using MoE_Sandbox.Virtualization.Filesystem.Host.NativeTypes;
using NLog;

namespace MoE_Sandbox.Virtualization.Filesystem
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
                                       $"Filename: {objectAttributes.ObjectName}, " +
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
            Logger.Log(LogLevel.Debug, "[NtOpenFile] " +
                                       $"Filename: {objectAttributes.ObjectName}, " +
                                       $"AccessMask: {access}, " +
                                       $"ShareOptions: {share}, " +
                                       $"OpenOptions: {openOptions}");

            var status = NativeApi.NtOpenFile(out handle, access, ref objectAttributes, out ioStatusBlock,
                share, openOptions);
            Logger.Log(LogLevel.Debug, $"[NtOpenFile] Status: {status}");

            return status;
        }
    }
}