using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TestErrorMessage
{
    class Program
    {
        const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000100;
        const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
        const int FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
        const int FORMAT_MESSAGE_FROM_STRING = 0x00000400;
        const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x000000FF;
        const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        static void Main(string[] args)
        {

        }

        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Auto)]
        internal static extern int FormatMessage(int flags, IntPtr source, int messageId, int languageId,
            StringBuilder buffer, int bufferSize, IntPtr argListPtr);

        internal static string GetMessage(int errorCode)
        {
            var sb = new StringBuilder(512);
            var returnCode = FormatMessage(FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero,
                errorCode, 0, sb, sb.Capacity, IntPtr.Zero);

            if (returnCode == 0) return $"Unknown Win32 error code {errorCode:x}";

            var message = sb.ToString();
            return message;
        }
    }
}
