using System;
using System.Collections.Generic;
using EasyHook;

namespace HookLibrary
{
    public class HookEntryPoint : IEntryPoint, IDisposable
    {
        private List<LocalHook> _filesystemHooks;

        private readonly object _callback;

        public void Initialize(RemoteHooking.IContext hookContext, string channelName)
        {
            throw new NotImplementedException();
        }

        public void Run(RemoteHooking.IContext hookContext, string channelName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
