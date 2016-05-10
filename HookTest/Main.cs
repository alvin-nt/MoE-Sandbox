using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;

namespace HookTest
{
    public class Main : IEntryPoint
    {
        static string ChannelName;
        RemoteMon Interface;

        LocalHook CreateFileAHook;
        LocalHook CreateFileWHook;

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr CreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        public delegate IntPtr TCreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr TCreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        static IntPtr hkCreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile)
        {
            try
            {
                ((Main)HookRuntimeInfo.Callback).Interface.GotFileNameA(filename);
                return CreateFileA(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
            }
            catch (Exception ex)
            {
                ((Main)HookRuntimeInfo.Callback).Interface.ErrorHandler(ex);
                return IntPtr.Zero;
            }
        }

        static IntPtr hkCreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile)
        {
            try
            {
                ((Main)HookRuntimeInfo.Callback).Interface.GotFileNameW(filename);
                return CreateFileW(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
            }
            catch (Exception ex)
            {
                ((Main)HookRuntimeInfo.Callback).Interface.ErrorHandler(ex);
                return IntPtr.Zero;
            }
        }

        public Main(RemoteHooking.IContext InContext, string InChannelName)
        {
            try
            {
                Interface = RemoteHooking.IpcConnectClient<RemoteMon>(InChannelName);
                ChannelName = InChannelName;
                Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            }
            catch (Exception ex)
            {
                Interface.ErrorHandler(ex);
            }
        }

        public void Run(RemoteHooking.IContext InContext, string InChannelName)
        {
            try
            {
                CreateFileWHook = LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileW"), new TCreateFileW(hkCreateFileW), this);
                CreateFileWHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateFileAHook = LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileA"), new TCreateFileA(hkCreateFileA), this);
                CreateFileAHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ex)
            {
                Interface.ErrorHandler(ex);
            }

            try
            {
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception ex)
            {
                Interface.ErrorHandler(ex);
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
