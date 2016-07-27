using System;
using System.Collections.Generic;
using System.Threading;
using EasyHook;
using HookLibrary.Filesystem;
using HookLibrary.Filesystem.Host;

namespace HookLibrary
{
    /// <summary>
    /// Entry point to the hook, which provides a 'bridge' between the ProcessManager and the process being hooked.
    /// </summary>
    /// <remarks>
    /// The <see cref="IEntryPoint"/> interface requires us to declare two methods:
    /// - Initialize(RemoteHooking.IContext, ...)
    /// - Run(RemoteHooking.IContext, ...)
    /// with (...) contains any parameters passed by the <see cref="RemoteHooking.CreateAndInject"/> method.
    /// These methods will be used by the EasyHook library as the entry point.
    /// </remarks>
    public class HookEntryPoint : IEntryPoint, IDisposable
    {
        protected List<LocalHook> FilesystemHooks;
        protected string ChannelName;

        protected FilesystemHook HookHandler;

        protected FilesystemRedirector RedirectorInterface;

        public HookEntryPoint(RemoteHooking.IContext hookContext, string channelName)
        {
            // connect to the redirector object.
            // this will be the object that handles the hook, from logging to redirection.
            // placed on the constructor, since readonly params can only be initialized from here.
            RedirectorInterface = RemoteHooking.IpcConnectClient<FilesystemRedirector>(channelName);
            HookHandler = new FilesystemHook(RedirectorInterface);
        }

        /// <summary>
        /// Prepares the hooks that are going to be injected to the target program.
        /// </summary>
        /// <param name="hookContext">Interface to the original process, which hooks the current process.</param>
        /// <param name="channelName">Unused. Placed, as the requirement of <see cref="IEntryPoint"/> interface.</param>
        public void Initialize(RemoteHooking.IContext hookContext, string channelName)
        {
        }

        /// <summary>
        /// Starts up the hooked process.
        /// </summary>
        /// <param name="hookContext">Unused.</param>
        /// <param name="channelName">Unused.</param>
        public void Run(RemoteHooking.IContext hookContext, string channelName)
        {
            RedirectorInterface.WriteToConsole($"Injected to {RemoteHooking.GetCurrentProcessId()}");

            try
            {
                // initialize the hooks
                FilesystemHooks = new List<LocalHook>
                {
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtCreateFile"),
                        new NativeApi.Delegates.NtCreateFile(HookHandler.NtCreateFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenFile"),
                        new NativeApi.Delegates.NtOpenFile(HookHandler.NtOpenFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtDeleteFile"),
                        new NativeApi.Delegates.NtDeleteFile(HookHandler.NtDeleteFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtQueryAttributesFile"),
                        new NativeApi.Delegates.NtQueryAttributesFile(HookHandler.NtQueryAttributesFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtQueryFullAttributesFile"),
                        new NativeApi.Delegates.NtQueryFullAttributesFile(HookHandler.NtQueryFullAttributesFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenSymbolicLinkObject"),
                        new NativeApi.Delegates.NtOpenSymbolicLinkObject(HookHandler.NtOpenSymbolicLinkObject),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenDirectoryObject"),
                        new NativeApi.Delegates.NtOpenDirectoryObject(HookHandler.NtOpenDirectoryObject),
                        this)
                };

                // set thread ACL to intercept only this thread.
                foreach (var hook in FilesystemHooks)
                {
                    hook.ThreadACL.SetExclusiveACL(new[] {0});
                }

                //Console.WriteLine($"Hooks are ready! The host's PID is {hookContext.HostPID}.");

                // notify that the hook is ready
                RedirectorInterface.WriteToConsole($"Hooks are ready! The host's PID is {hookContext.HostPID}.");
            }
            catch (Exception e)
            {
                RedirectorInterface.WriteToConsole(e.ToString());
            }

            // let's start the process!
            try
            {
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception e)
            {
                RedirectorInterface.WriteToConsole(e.ToString());
                return; // cannot do anything at this point, so just return.. or throw an unhandled exception
            }

            // The IPC channel is still up and running; just block this thread.
            // When the thread exits from this method, we do not need
            // TODO: find a better way for this thread not to return, instead of just blocking this thread.
            while (RedirectorInterface.Ping())
            {
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            foreach (var hook in FilesystemHooks)
            {
                try
                {
                    hook.Dispose();
                }
                catch (Exception)
                {
                    // silently ignore
                }
            }
        }
    }
}