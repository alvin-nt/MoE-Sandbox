using System;

namespace HookTest
{
    public class RemoteMon : MarshalByRefObject
    {
        public bool Ping()
        {
            return true;
        }

        public void IsInstalled(int clientPid)
        {
            Log($"Successfully injected into PID {clientPid}.");
        }
        
        public void GotFileNameW(string fileName)
        {
            Log($"[CreateFileW] Opened file: {fileName}.");
        }

        public void GotFileNameA(string fileName)
        {
            Log($"[CreateFileA] Opened file: {fileName}.");
        }

        public void ErrorHandler(Exception ex)
        {
            Log(ex.ToString());
        }

        public void Log(string text)
        {
            Console.WriteLine($"[{DateTime.Now}] {text}");
        }
    }
}
