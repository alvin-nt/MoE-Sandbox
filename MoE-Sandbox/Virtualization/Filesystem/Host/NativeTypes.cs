using System;
using System.IO;
using System.Runtime.InteropServices;
using MoE_Sandbox.Virtualization.Filesystem.Host.NativeTypes;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace MoE_Sandbox.Virtualization.Filesystem.Host
{
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
    /// From MSDN:
    /// The <see cref="SecurityQos"/> data structure contains information used to support client impersonation.
    /// A client can specify this information when it connects to a server; the information determines whether 
    /// the server may impersonate the client, and if so, to what extent.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityQos
    {
        public uint Length;

        [MarshalAs(UnmanagedType.I4)] public int ImpersonationLevel;

        internal byte ContextDynamicTrackingMode;

        internal byte EffectiveOnly;
    }

    /// <summary>
    /// Represents Unicode string, that is used by the system interop services.
    /// Contains code from http://www.pinvoke.net/default.aspx/Structures/UNICODE_STRING.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
    public struct UnicodeString : IDisposable
    {
        public ushort Length;
        public ushort MaximumLength;
        private readonly IntPtr _buffer;

        public UnicodeString(string s)
        {
            Length = (ushort) (s.Length*2);
            MaximumLength = (ushort) (Length + 2);
            _buffer = Marshal.StringToHGlobalUni(s);
        }

        public override string ToString()
        {
            return Marshal.PtrToStringUni(_buffer) ?? "";
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_buffer);
        }
    }

    /// <summary>
    /// Used for marking I/O Request Package (IRP) status
    /// Documentation from MSDN:
    /// https://msdn.microsoft.com/en-us/library/windows/hardware/ff550671(v=vs.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct IoStatusBlock
    {
        /// <summary>
        /// This is the completion status, either <see cref="NtStatusCode.Success"/> if the requested operation was completed successfully
        /// or an informational, warning, or error <see cref="NtStatusCode"/> value.
        /// </summary>
        public uint Status;

        /// <summary>
        /// This is set to a request-dependent value.
        /// For example, on successful completion of a transfer request, this is set to the number of bytes transferred.
        /// If a transfer request is completed with another <see cref="NtStatusCode"/>, this member is set to zero.
        /// </summary>
        public IntPtr Information;
    }

    /// <summary>
    /// Represents basic information about a file.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FileBasicInformation
    {
        /// <summary>
        /// The time the file was created as native Windows <see cref="FileTime"/> format.
        /// </summary>
        [FieldOffset(0)] internal FileTime _creationTime;

        /// <summary>
        /// The time the file was accessed as native Windows <see cref="FileTime"/> format.
        /// </summary>
        [FieldOffset(8)] internal FileTime _lastAccessTime;

        /// <summary>
        /// The time the file was written as native Windows <see cref="FileTime"/> format.
        /// </summary>
        [FieldOffset(16)] internal FileTime _lastWriteTime;

        /// <summary>
        /// The time the file was changed as native Windows <see cref="FileTime"/> format.
        /// </summary>
        [FieldOffset(24)] internal FileTime _changeTime;

        /// <summary>
        /// Attributes of the file.
        /// </summary>
        [FieldOffset(32)] public FileAttributes FileAttributes;

        /// <summary>
        /// The time the file was created as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime CreationTime
        {
            get { return Utils.DateTimeFromFileTime(_creationTime); }
            set { _creationTime = Utils.FileTimeFromDateTime(value); }
        }

        /// <summary>
        /// The time the file was accessed as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return Utils.DateTimeFromFileTime(_lastAccessTime); }
            set { _lastAccessTime = Utils.FileTimeFromDateTime(value); }
        }

        /// <summary>
        /// The time the file was last written to as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return Utils.DateTimeFromFileTime(_lastWriteTime); }
            set { _lastWriteTime = Utils.FileTimeFromDateTime(value); }
        }

        /// <summary>
        /// The time the file was changed as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime ChangeTime
        {
            get { return Utils.DateTimeFromFileTime(_changeTime); }
            set { _changeTime = Utils.FileTimeFromDateTime(value); }
        }
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