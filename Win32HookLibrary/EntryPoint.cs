using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHook;

namespace Win32HookLibrary
{
    public class EntryPoint : IEntryPoint, IDisposable
    {
        private List<LocalHook> _hooks;
        private string _channelName;



        public EntryPoint(RemoteHooking.IContext context, string channelName)
        {
            
        }

        public void Initialize(RemoteHooking.IContext context, string channelName)
        {
            
        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
