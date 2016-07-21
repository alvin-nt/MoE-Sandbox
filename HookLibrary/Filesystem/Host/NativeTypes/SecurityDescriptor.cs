using System;
using System.Runtime.InteropServices;

namespace HookLibrary.Filesystem.Host.NativeTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityDescriptor
    {
        public byte revision;
        public byte size;
        public short control;
        public IntPtr owner;
        public IntPtr group;

        /// <summary>
        /// System ACL
        /// </summary>
        public IntPtr sacl;

        /// <summary>
        /// Directory ACL
        /// </summary>
        public IntPtr dacl;
    }
}