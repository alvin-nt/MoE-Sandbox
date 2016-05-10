using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HookTest
{
    public class RemoteMon : MarshalByRefObject
    {
        public void IsInstalled(int InClientPID)
        {
            Console.WriteLine("Successfully injected into PID {0}.", InClientPID);
        }
        
        public void GotFileNameW(String FileName)
        {
            Console.WriteLine("[CreateFileW] Opened file: {0}.", FileName);
        }

        public void GotFileNameA(String FileName)
        {
            Console.WriteLine("[CreateFileA] Opened file: {0}.", FileName);
        }

        public void ErrorHandler(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
