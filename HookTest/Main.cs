using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;

namespace HookTest
{
    public class Main : IEntryPoint, IDisposable
    {
        string ChannelName;
        readonly RemoteMon Interface;

        LocalHook CreateFileAHook;
        LocalHook CreateFileWHook;

        protected readonly List<LocalHook> Hooks;

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
                ((Main) HookRuntimeInfo.Callback).Interface.GotFileNameA(filename);
                return CreateFileA(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes,
                    templateFile);
            }
            catch (Exception ex)
            {
                ((Main) HookRuntimeInfo.Callback).Interface.ErrorHandler(ex);
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
                ((Main) HookRuntimeInfo.Callback).Interface.GotFileNameW(filename);
                return CreateFileW(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes,
                    templateFile);
            }
            catch (Exception ex)
            {
                ((Main) HookRuntimeInfo.Callback).Interface.ErrorHandler(ex);
                return IntPtr.Zero;
            }
        }

        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                Interface = RemoteHooking.IpcConnectClient<RemoteMon>(inChannelName);
                ChannelName = inChannelName;
                Initialize(inContext, inChannelName);

                Hooks = new List<LocalHook>(2);
            }
            catch (Exception ex)
            {
                Interface.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Initializes the hook: assigns all passed parameters from the <see cref="RemoteHooking"/>
        /// </summary>
        /// <param name="inContext">the hook context</param>
        /// <param name="inChannelName">Contains the name of the .NET Remoting 'channel' to connect to</param>
        public void Initialize(RemoteHooking.IContext inContext, string inChannelName)
        {
            Interface?.IsInstalled(RemoteHooking.GetCurrentProcessId());
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                Hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileW"),
                    new TCreateFileW(hkCreateFileW), this));
                Hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileA"),
                    new TCreateFileA(hkCreateFileA), this));

                foreach (var hook in Hooks)
                {
                    hook.ThreadACL.SetExclusiveACL(null);
                }
            }
            catch (Exception ex)
            {
                Interface?.ErrorHandler(ex);

                return;
            }

            try
            {
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception ex)
            {
                Interface?.ErrorHandler(ex);

                return;
            }

            while (Interface.Ping())
            {
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            foreach (var hook in Hooks)
            {
                try
                {
                    hook.Dispose();
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}