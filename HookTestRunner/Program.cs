using System;
using System.IO;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.Threading;
using EasyHook;
using HookTest;

namespace HookTestRunner
{
    class Program
    {
        public static string ChannelName = null;

        public static string CurrentDir =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            int processId = -1;
            try
            {
                RemoteHooking.IpcCreateServer<RemoteMon>(
                    ref ChannelName, WellKnownObjectMode.Singleton);

                Console.Write("Please type your process name here: ");
                string processName = Console.ReadLine();

                foreach (Process p in Process.GetProcessesByName(processName))
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
                    CurrentDir + "HookTest.dll", new object[] {ChannelName});
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error while connecting to target: " + Environment.NewLine + "{0}",
                    ex.ToString());
                Console.ReadLine();
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            
        }
    }
}