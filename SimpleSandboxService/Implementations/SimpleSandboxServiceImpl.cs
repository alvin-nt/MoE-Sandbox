using System;
using NLog;

namespace SimpleSandboxService.Implementations
{
    public partial class SimpleSandboxServiceImpl : IDisposable
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public bool Reset()
        {
            ResetWindowsUpdate();
            return ResetRegistry();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
