using System;
using System.IO;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace HookLibrary.Filesystem.Host.NativeTypes
{
    /// <summary>
    /// Represents basic information about a file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FileBasicInformation
    {
        /// <summary>
        /// The time the file was created as native Windows <see cref="FILETIME"/> format.
        /// </summary>
        private FILETIME _creationTime;

        /// <summary>
        /// The time the file was accessed as native Windows <see cref="FILETIME"/> format.
        /// </summary>
        private FILETIME _lastAccessTime;

        /// <summary>
        /// The time the file was written as native Windows <see cref="FILETIME"/> format.
        /// </summary>
        private FILETIME _lastWriteTime;

        /// <summary>
        /// The time the file was changed as native Windows <see cref="FILETIME"/> format.
        /// </summary>
        private FILETIME _changeTime;

        /// <summary>
        /// Attributes of the file.
        /// </summary>
        public FileAttributes FileAttributes;

        /// <summary>
        /// The time the file was created as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime CreationTime
        {
            get { return _creationTime.ToDateTime(); }
            set { _creationTime = value.ToNativeFileTime(); }
        }

        /// <summary>
        /// The time the file was accessed as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return _lastAccessTime.ToDateTime(); }
            set { _lastAccessTime = value.ToNativeFileTime(); }
        }

        /// <summary>
        /// The time the file was last written to as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return _lastWriteTime.ToDateTime(); }
            set { _lastWriteTime = value.ToNativeFileTime(); }
        }

        /// <summary>
        /// The time the file was changed as <see cref="DateTime"/> object.
        /// </summary>
        public DateTime ChangeTime
        {
            get { return _changeTime.ToDateTime(); }
            set { _changeTime = value.ToNativeFileTime(); }
        }

        public override string ToString()
        {
            return $"Attributes: {FileAttributes}, " +
                   $"CreationTime: {CreationTime}, " +
                   $"LastAccessTime: {LastAccessTime}, " +
                   $"LastWriteTime: {LastWriteTime}, " +
                   $"ChangeTime: {ChangeTime}";
        }
    }
}