using System;
using System.Runtime.InteropServices;

namespace HookLibrary.Filesystem.Host.NativeTypes
{
    // NOTE: you may refactor this file, as needed

    /// <summary>
    /// An action to take on a file or device that exists or does not exist. 
    /// For devices other than files, this parameter is usually set to OPEN_EXISTING.
    /// </summary>
    public enum NtFileCreateDisposition : uint
    {
        /// <summary>
        /// If the file already exists, replace it with the given file.
        /// If it does not, create the given file.
        /// </summary>
        Supersede = 0,

        /// <summary>
        /// If the specified file exists, the function fails.
        /// If the specified file does not exist and is a valid path to a writable location, a new file is created.
        /// </summary>
        Create = 1,

        /// <summary>
        /// If the file already exists, open it instead of creating a new file.
        /// If it does not, fail the request and do not create a new file.
        /// </summary>
        Open = 2,

        /// <summary>
        /// If the file already exists, open it. If it does not, create the given file.
        /// </summary>
        OpenIf = 3,

        /// <summary>
        /// If the file already exists, open it and overwrite it. If it does not, fail the request.
        /// </summary>
        Overwrite = 4,

        /// <summary>
        /// If the file already exists, open it and overwrite it. If it does not, create the given file.
        /// </summary>
        OverwriteIf = 5
    }

    /// <summary>
    /// Represents the file creation options imposed on the system call.
    /// </summary>
    /// <remarks>
    /// TODO: some values are not captured.
    /// </remarks>
    [Flags]
    public enum NtFileOptions : uint
    {
        WriteThrough = 0x00000002,
        SequentialOnly = 0x00000004,
        NoImmediateBuffering = 0x00000008,
        SynchronousIoNonAlert = 0x00000020,
        RandomAccess = 0x00000800
    }

    /// <summary>
    /// Contains information used to support client impersonation.
    /// A client can specify this information when it connects to a server; the information determines whether 
    /// the server may impersonate the client, and if so, to what extent.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityQos
    {
        public uint Length;

        public SecurityImpersonationLevel ImpersonationLevel;

        public byte ContextDynamicTrackingMode;

        public byte EffectiveOnly;
    }

    public enum SecurityImpersonationLevel
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    }

    /// <summary>
    /// Used for marking I/O Request Package (IRP) status.
    /// </summary>
    /// <remarks>
    /// Source: https://msdn.microsoft.com/en-us/library/windows/hardware/ff550671(v=vs.85).aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct IoStatusBlock
    {
        /// <summary>
        /// This is the completion status, either <see cref="NtStatus.Success"/> if the requested 
        /// operation was completed successfully or an informational, warning, or error 
        /// <see cref="NtStatus"/> value.
        /// </summary>
        public NtStatus Status;

        /// <summary>
        /// This is set to a request-dependent value.
        /// For example, on successful completion of a transfer request, this is set to the number of bytes transferred.
        /// If a transfer request is completed with another <see cref="NtStatus"/>, this member is set to zero.
        /// </summary>
        public IntPtr Information;
    }

    /// <summary>
    /// Contains information about the extended attributes of a file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct FileFullEaInformation
    {
        public uint NextEntryOffset;
        public byte Flags;
        public byte EaNameLength;
        public ushort EaValueLength;
        public byte EaName;
    }
}