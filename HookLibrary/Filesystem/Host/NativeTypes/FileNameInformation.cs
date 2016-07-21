using System;
using System.Runtime.InteropServices;

namespace HookLibrary.Filesystem.Host.NativeTypes
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
    public struct FileNameInformation
    {
        public uint Length;

        private readonly IntPtr _fileName;

        public string FileName => Marshal.PtrToStringUni(_fileName, (int)Length) ?? "";

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}