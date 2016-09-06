using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using EasyHook;
using System.Windows.Forms;

namespace HookTest
{
    public class Main : IEntryPoint, IDisposable
    {
        public readonly RemoteMon Interface;

        private readonly List<LocalHook> _hooks;

        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                Interface = RemoteHooking.IpcConnectClient<RemoteMon>(inChannelName);
                Initialize(inContext, inChannelName);

                _hooks = new List<LocalHook>(10);
            }
            catch (Exception ex)
            {
                Interface?.LogError(ex);
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
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileA"),
                    new NativeApi.Delegates.CreateFileA(FilesystemHookHandler.CreateFileA),
                    this));
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateFileW"),
                    new NativeApi.Delegates.CreateFileW(FilesystemHookHandler.CreateFileW),
                    this));

                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateDirectoryA"),
                    new NativeApi.Delegates.CreateDirectoryA(FilesystemHookHandler.CreateDirectoryA),
                    this));
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "CreateDirectoryW"),
                    new NativeApi.Delegates.CreateDirectoryW(FilesystemHookHandler.CreateDirectoryW),
                    this));

                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "DeleteFileA"),
                    new NativeApi.Delegates.DeleteFileA(FilesystemHookHandler.DeleteFileA),
                    this));
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "DeleteFileW"),
                    new NativeApi.Delegates.DeleteFileW(FilesystemHookHandler.DeleteFileW),
                    this));

                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "LoadLibraryExA"),
                    new NativeApi.Delegates.LoadLibraryExA(FilesystemHookHandler.LoadLibraryExA),
                    this));
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "LoadLibraryExW"),
                    new NativeApi.Delegates.LoadLibraryExW(FilesystemHookHandler.LoadLibraryExW),
                    this));

                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "RemoveDirectoryA"),
                    new NativeApi.Delegates.RemoveDirectoryA(FilesystemHookHandler.RemoveDirectoryA),
                    this));
                _hooks.Add(LocalHook.Create(LocalHook.GetProcAddress("Kernel32.dll", "RemoveDirectoryW"),
                    new NativeApi.Delegates.RemoveDirectoryW(FilesystemHookHandler.RemoveDirectoryW),
                    this));

                foreach (var hook in _hooks)
                {
                    // set up the hooks to hook all thread
                    hook.ThreadACL.SetExclusiveACL(null);
                }
            }
            catch (Exception ex)
            {
                Interface?.LogError(ex);

                return; // cannot continue with the hook; just dispose.
            }

#if DEBUG
            var messageBoxResult =
                MessageBox.Show($"Do you want to attach a debugger to process {RemoteHooking.GetCurrentProcessId()}?",
                    "Attach Debugger",
                    MessageBoxButtons.YesNo);
            if (messageBoxResult == DialogResult.Yes)
            {
                Debugger.Launch();
            }
#endif

            try
            {
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception ex)
            {
                Interface?.LogError(ex);

                return;
            }

            try
            {
                while (Interface.Ping())
                {
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                // if disconnected, interface will throw an exception.
                // silently return, so that the hooks can be disposed.
            }
        }

        public void Dispose()
        {
            foreach (var hook in _hooks)
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