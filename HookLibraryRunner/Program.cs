using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using EasyHook;
using HookLibrary.Filesystem;

namespace HookLibraryRunner
{
    class Program
    {
        static string ChannelName = null;
        private static IpcServerChannel _channel;

        public static string CurrentDir =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            var processId = -1;
            try
            {
                _channel = RemoteHooking.IpcCreateServer<FilesystemRedirector>(
                    ref ChannelName, WellKnownObjectMode.Singleton);

                Console.WriteLine($"Created .NET Remoting server on channel {ChannelName}");

                Console.Write("Please type your process name here: ");
                var processName = Console.ReadLine();

                foreach (var p in Process.GetProcessesByName(processName))
                {
                    processId = p.Id;
                    break;
                }

                if (processId == -1)
                {
                    Console.WriteLine("No process exists with that name!");
                    Console.ReadLine();
                    return;
                }
                RemoteHooking.Inject(processId, InjectionOptions.DoNotRequireStrongName, CurrentDir + "HookLibrary.dll",
                    CurrentDir + "HookLibrary.dll", ChannelName);

                _channel.StartListening(null);
                Console.WriteLine("Injected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error while connecting to target: " + Environment.NewLine + "{0}",
                    ex);
                Console.ReadLine();
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _channel.StopListening(null);
        }
    }
}
