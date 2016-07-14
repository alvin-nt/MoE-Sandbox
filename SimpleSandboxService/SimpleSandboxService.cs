using System;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using WindowsUpdateControllerService;
using NLog;
using RegistryService;
using SimpleSandboxService.Implementations;

namespace SimpleSandboxService
{
    public partial class SimpleSandboxService : ServiceBase
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        protected ServiceHost Host;

        public SimpleSandboxService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // initialize the host
            Host = new ServiceHost(typeof(SimpleSandboxServiceImpl), new Uri("net.pipe://localhost"));

            Host.AddServiceEndpoint(typeof(IRegistryService),
                new NetNamedPipeBinding(),
                ServiceName);
            Host.AddServiceEndpoint(typeof(IWindowsUpdateControllerService),
                new NetNamedPipeBinding(),
                ServiceName);

            using (var service = new SimpleSandboxServiceImpl())
            {
                while (!service.Reset())
                {
                    Thread.Sleep(1000);
                }
            }
        }

        protected override void OnStop()
        {
            try
            {
                Host?.Close();

                using (var service = new SimpleSandboxServiceImpl())
                {
                    service.Reset();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Cannot stop {ServiceName} service!");
            }
        }
    }
}
