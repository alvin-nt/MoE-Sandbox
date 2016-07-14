﻿using System.ServiceProcess;

namespace SimpleSandboxService
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new SimpleSandboxService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
