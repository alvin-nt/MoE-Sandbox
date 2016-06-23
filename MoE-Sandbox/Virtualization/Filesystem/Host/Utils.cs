using System;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace MoE_Sandbox.Virtualization.Filesystem.Host
{
    /// <summary>
    /// Contains utility functions to manipulate Windows datatypes.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Converts native Windows <see cref="FileTime"/> (from Windows native types) to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="filetime">The <see cref="FileTime"/> variable to be converted.</param>
        /// <returns><see cref="DateTime"/> object.</returns>
        public static DateTime DateTimeFromFileTime(FileTime filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;

            return DateTime.FromFileTimeUtc(highBits | (uint) filetime.dwLowDateTime);
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> to native Windows <see cref="FileTime"/>.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> object to be converted.</param>
        /// <returns><see cref="FileTime"/> object.</returns>
        public static FileTime FileTimeFromDateTime(DateTime time)
        {
            var converted = new FileTime();
            var origin = time.ToFileTime();

            converted.dwHighDateTime = (int) (origin & 0xFFFFFFFF);
            converted.dwLowDateTime = (int) (origin >> 32);

            return converted;
        }
    }
}