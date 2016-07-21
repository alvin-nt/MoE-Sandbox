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
    public struct SafeUnicodeString : IDisposable
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
        /// Create a <see cref="SafeUnicodeString"/> from a <see cref="string"/> object.
        /// </summary>
        /// <param name="str"><see cref="string"/> object to be converted.</param>
        public SafeUnicodeString(string str)
        {
            Length = (ushort) (str.Length * UnicodeEncoding.CharSize);
            MaximumLength = (ushort) (Length + UnicodeEncoding.CharSize); // add 1 char for NULL
            _buffer = Marshal.StringToHGlobalUni(str);
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
}