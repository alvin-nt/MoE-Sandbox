using System;
using System.IO;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.Threading;
using EasyHook;
using HookTest;
using System.Reflection;
using System.Runtime.Remoting.Channels.Ipc;

namespace HookTestRunner
{
    class Program
    {
        public static string ChannelName = null;
        private static IpcServerChannel _channel;

        public static string CurrentDir =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            int processId = -1;
            try
            {
                _channel = RemoteHooking.IpcCreateServer<RemoteMon>(
                    ref ChannelName, WellKnownObjectMode.Singleton);

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
                RemoteHooking.Inject(processId, InjectionOptions.DoNotRequireStrongName, CurrentDir + "HookTest.dll",
                    CurrentDir + "HookTest.dll", ChannelName);
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