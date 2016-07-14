using System;
using System.ServiceProcess;
using WindowsUpdateControllerService;
using OsInfo;
using OsInfo.Extensions;
using WUApiLib;

namespace SimpleSandboxService.Implementations
{
    public partial class SimpleSandboxServiceImpl : IWindowsUpdateControllerService
    {
        protected const string Win10WindowsUpdateServiceName = "wuauserv";

        /// <summary>
        /// Wrapper for getting/setting Windows Update service status.
        /// </summary>
        protected bool WindowsUpdateStatus
        {
            get
            {
                if (Environment.OSVersion.IsEqualTo(OsVersion.Win10))
                {
                    using (var service = new ServiceController(Win10WindowsUpdateServiceName))
                        return service.Status.Equals(ServiceControllerStatus.Running);
                }

                {
                    // use WUApiLib
                    var auc = new AutomaticUpdatesClass();
                    return auc.ServiceEnabled;
                }
            }

            set
            {
                // just ignore if the value is same as the current state
                if (value == WindowsUpdateStatus) return;

                try
                {
                    if (Environment.OSVersion.IsEqualTo(OsVersion.Win10))
                    {
                        // Stop Windows Service on Windows 10
                        using (var service = new ServiceController(Win10WindowsUpdateServiceName))
                        {
                            if (value)
                                service.Start();
                            else
                                service.Stop();
                        }
                    }
                    else
                    {
                        // For lower version of Windows, use WUApiLib
                        var auc = new AutomaticUpdatesClass();

                        if (value)
                            auc.Resume();
                        else
                            auc.Pause();
                    }

                    Logger.Info($"Windows Update is {(value ? "enabled" : "disabled")}.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Cannot change Windows Update status to {(value ? "enabled" : "disabled")}.");
                    throw;
                }
            }
        }

        public bool Ping()
        {
            return true;
        }

        public bool IsWindowsUpdateEnabled()
        {
            return WindowsUpdateStatus;
        }

        public void SetWindowsUpdateService(bool enabled)
        {
            WindowsUpdateStatus = enabled;
        }

        public void ResetWindowsUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
