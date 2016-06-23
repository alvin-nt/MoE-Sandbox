using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHook;
using MoE_Sandbox.Virtualization.Filesystem;
using MoE_Sandbox.Virtualization.Filesystem.Host;

namespace MoE_Sandbox.Virtualization
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
