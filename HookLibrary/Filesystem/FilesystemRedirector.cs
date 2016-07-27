using System;
using NLog;

namespace HookLibrary.Filesystem
{
    public class FilesystemRedirector : MarshalByRefObject
    {
        /// <summary>
        /// Logger interface that is going to be used by the invoking hook functions.
        /// </summary>
        // TODO: fix this, not working :(
        public readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Path to the root of folder, in which all calls would be redirected to.
        /// </summary>
        protected readonly string _basePath;

        public FilesystemRedirector()
        {
            // TODO: do the actual redirection stuff
            _basePath = "";
            //Logger.Debug("Logger initialized!");
        }

        public bool Ping()
        {
            return true;
        }

        public void WriteToConsole(string s)
        {
            Console.WriteLine($"[{DateTime.Now}] {s}");
        }

        // TODO: do the actual redirection stuff
    }
}
