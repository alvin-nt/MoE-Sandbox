using System;
using System.Runtime.InteropServices.ComTypes;

namespace HookLibrary.Filesystem.Host
{
    /// <summary>
    /// Contains extension methods to manipulate Windows native datatypes.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Converts native Windows <see cref="FILETIME"/> (from Windows native types) to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="filetime">The <see cref="FILETIME"/> variable to be converted.</param>
        /// <returns><see cref="DateTime"/> object.</returns>
        public static DateTime ToDateTime(this FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;

            try
            {
                return DateTime.FromFileTimeUtc(highBits | (uint)filetime.dwLowDateTime);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(0);
            }
        }

        /// <summary>
        ///     Converts <see cref="DateTime"/> to native Windows <see cref="FILETIME"/> object.
        /// </summary>
        /// <param name="filetime">Reference to <see cref="FILETIME"/> class.</param>
        /// <param name="time">The <see cref="DateTime"/> object to be converted.</param>
        /// <returns><see cref="FILETIME"/> object.</returns>
        public static FILETIME FromDateTime(this FILETIME filetime, DateTime time)
        {
            return time.ToNativeFileTime();
        }

        /// <summary>
        ///     Converts <see cref="DateTime"/> to native Windows <see cref="FILETIME"/>.
        /// </summary>
        /// <param name="time">Reference to <see cref="DateTime"/> class.</param>
        /// <remarks>
        ///     Need to add 'native' at the beginning, since <see cref="DateTime.ToFileTime"/> has been used by the system.
        /// </remarks>
        /// <returns><see cref="FILETIME"/> object.</returns>
        public static FILETIME ToNativeFileTime(this DateTime time)
        {
            var origin = time.ToFileTime();

            return new FILETIME
            {
                dwHighDateTime = (int)(origin & 0xFFFFFFFF),
                dwLowDateTime = (int)(origin >> 32)
            };
        }

        /// <summary>
        ///     Converts native Windows <see cref="FILETIME"/> to <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="time">Reference to <see cref="DateTime"/> class.</param>
        /// <param name="filetime"><see cref="FILETIME"/> object to be converted.</param>
        /// <remarks>
        ///     Need to add 'native' at the beginning, since <see cref="DateTime.ToFileTime"/> has been used by the system.
        /// </remarks>
        /// <returns><see cref="DateTime"/> object.</returns>
        public static DateTime FromNativeFileTime(this DateTime time, FILETIME filetime)
        {
            return filetime.ToDateTime();
        }
    }
}
