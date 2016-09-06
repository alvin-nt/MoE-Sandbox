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

        public void Log(string text)
        {
            Console.WriteLine($"[{DateTime.Now}] {text}");
        }

        public void LogError(Exception ex)
        {
            Log($"[{ex.GetType()}] {ex}");
        }
    }
}
