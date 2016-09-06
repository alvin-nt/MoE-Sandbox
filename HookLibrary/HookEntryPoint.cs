using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using EasyHook;
using HookLibrary.Filesystem;
using HookLibrary.Filesystem.Host;
using NLog;

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

        protected LocalHook NtCreateFileHook;

        protected string ChannelName;

        protected readonly FilesystemRedirector RedirectorInterface;

        public readonly Queue<Tuple<LogLevel, string>> Logs;

        public HookEntryPoint(RemoteHooking.IContext hookContext, string channelName)
        {
            // connect to the redirector object.
            // this will be the object that handles the hook, from logging to redirection.
            // placed on the constructor, since readonly params can only be initialized from here.
            RedirectorInterface = RemoteHooking.IpcConnectClient<FilesystemRedirector>(channelName);
            Logs = new Queue<Tuple<LogLevel, string>>(2048);

            Initialize(hookContext, channelName);
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
            RedirectorInterface.WriteToConsole($"Injected to PID {RemoteHooking.GetCurrentProcessId()}.");

            try
            {
                // initialize the hooks
                FilesystemHooks = new List<LocalHook>
                {
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtCreateFile"),
                        new NativeApi.Delegates.NtCreateFile(FilesystemHookHandler.NtCreateFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenFile"),
                        new NativeApi.Delegates.NtOpenFile(FilesystemHookHandler.NtOpenFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtDeleteFile"),
                        new NativeApi.Delegates.NtDeleteFile(FilesystemHookHandler.NtDeleteFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtQueryAttributesFile"),
                        new NativeApi.Delegates.NtQueryAttributesFile(FilesystemHookHandler.NtQueryAttributesFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtQueryFullAttributesFile"),
                        new NativeApi.Delegates.NtQueryFullAttributesFile(FilesystemHookHandler.NtQueryFullAttributesFile),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenSymbolicLinkObject"),
                        new NativeApi.Delegates.NtOpenSymbolicLinkObject(FilesystemHookHandler.NtOpenSymbolicLinkObject),
                        this),
                    LocalHook.Create(LocalHook.GetProcAddress("ntdll.dll", "NtOpenDirectoryObject"),
                        new NativeApi.Delegates.NtOpenDirectoryObject(FilesystemHookHandler.NtOpenDirectoryObject),
                        this)
                };

                // set thread ACL to intercept all threads.
                foreach (var hook in FilesystemHooks)
                {
                    hook.ThreadACL.SetExclusiveACL(null);
                }
            }
            catch (Exception e)
            {
                RedirectorInterface.WriteToConsole(e.ToString());
                return;
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

            // let's start the process!
            try
            {
                RemoteHooking.WakeUpProcess();

                RedirectorInterface.WriteToConsole($"Hooks are ready! The host's PID is {hookContext.HostPID}.");
            }
            catch (Exception e)
            {
                RedirectorInterface.WriteToConsole(e.ToString());
                return; // cannot do anything at this point, so just return.. or throw an unhandled exception
            }

            // Periodically flush the logs
            try
            {
                while (RedirectorInterface.Ping())
                {
                    Thread.Sleep(500);

                    var logItems = new Tuple<LogLevel, string>[0];
                    bool flush;

                    // flush the log
                    lock (Logs)
                    {
                        flush = Logs.Count > 0;
                        if (flush)
                        {
                            logItems = Logs.ToArray();
                            Logs.Clear();
                        }
                    }

                    if (!flush) continue;
                    foreach (var logItem in logItems)
                    {
                        //RedirectorInterface.Logger.Log(logItem.Item1, logItem.Item2);
                        RedirectorInterface.WriteToConsole(logItem.Item2);
                    }
                }
            }
            catch
            {
                // Ping() will throw exception when disconnected.
                // just silently return, so that we can continue the invocation.
            }
        }

        public void AddToLogQueue(LogLevel logLevel, string log)
        {
            lock (Logs)
            {
                Logs.Enqueue(new Tuple<LogLevel, string>(logLevel, log));
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
                catch
                {
                    // silently ignore
                }
            }
        }
    }
}