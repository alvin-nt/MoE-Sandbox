using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HookLibrary.Filesystem.Host.NativeTypes
{
    /// <summary>
    /// Represents Unicode string, that is used by the system interop services.
    /// This class is written without pointers, so that buffer overrun/overflow does not occur.   
    /// </summary>
    /// <remarks>
    ///     Source: http://www.pinvoke.net/default.aspx/Structures/UNICODE_STRING.html
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
    public struct UnicodeString : IDisposable
    {
        /// <summary>
        /// Length of the string.
        /// </summary>
        public readonly ushort Length;

        /// <summary>
        /// Maximum length of the string.
        /// </summary>
        public readonly ushort MaximumLength;

        /// <summary>
        /// Pointer to unmanaged block of memory that stores the characters.
        /// </summary>
        private readonly IntPtr _buffer;

        /// <summary>
        /// Create a <see cref="UnicodeString"/> from a <see cref="string"/> object.
        /// </summary>
        /// <param name="str"><see cref="string"/> object to be converted.</param>
        public UnicodeString(string str)
        {
            Length = (ushort) (str.Length * UnicodeEncoding.CharSize);
            MaximumLength = (ushort) (Length + UnicodeEncoding.CharSize); // add 1 char for NULL
            _buffer = Marshal.StringToHGlobalUni(str);
        }

        public override string ToString()
        {
            return Marshal.PtrToStringUni(_buffer) ?? "";
        }

        /// <summary>
        /// Get the string version of the <see cref="UnicodeString"/>, without the '\??\' prefix.
        /// </summary>
        /// <returns>Unicode string, without prefix.</returns>
        /// <remarks>
        /// <seealso cref="HasPrefix"/> has information for the meaning of '\??\' prefix.
        /// </remarks>
        public string WithoutPrefix()
        {
            var original = ToString();

            return original.StartsWith(@"\??\") ? original.Substring(4) : original;
        }

        /// <summary>
        /// Checks whether the <see cref="UnicodeString"/> has the '\??\' prefix.
        /// </summary>
        /// <returns>true if the prefix exists</returns>
        /// <remarks>
        /// For file I/O, the "\\?\" prefix to a path string tells the Windows APIs to disable all string parsing and to send the string that follows it straight to the file system.
        /// </remarks>
        public bool HasPrefix()
        {
            return ToString().StartsWith(@"\??\");
        }

        public bool IsDosPath()
        {
            var original = WithoutPrefix();

            return original.Length > 2 && original[1] == ':';
        }

        public bool IsNtPath()
        {
            return WithoutPrefix().StartsWith("\\");
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_buffer);
        }

        public static implicit operator string(UnicodeString s)
        {
            return s.ToString();
        }
    }
}