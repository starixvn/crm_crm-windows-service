// -----------------------------------------------------------------------
// <copyright file="SystemService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Fanex.CRM.CRMReplication.Core;
    using Fanex.CRM.CRMReplication.Core.Helper;
    using System.Threading;
    using System.ServiceProcess;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SystemService:ISystemService
    {
        public void CheckMemoryUsage()
        {

            double f = 1024.0;
            Process proc = Process.GetCurrentProcess();
            var procName = proc.ProcessName;
            var privateMemory = proc.PrivateMemorySize64 / f;

            if (privateMemory > AppSettings.MemoryUsageLimit)
            {
                Logger.WriteLog(string.Format("{0}: Memory used is {1}, it is larger than {2}.", procName, privateMemory, AppSettings.MemoryUsageLimit));
                
            }
            else
                Logger.WriteLog(string.Format("{0}: Memory used is {1} KB.", procName, privateMemory));
        }

        private static void RestartService(string serviceName)
        {

            ServiceController controller = new ServiceController(serviceName);
            if (controller.Status == ServiceControllerStatus.Running)
                controller.Stop();
            //Thread.Sleep(2000);
            controller.Refresh();
            //ServiceController controller2 = new ServiceController(serviceName);
            if (controller.Status == ServiceControllerStatus.StopPending | controller.Status == ServiceControllerStatus.Stopped) {
                controller.Refresh();
                ServiceController controller1 = new ServiceController(serviceName);
                var s = controller1.Status;
                controller1.Start(); 
            }
                
        }

    }
}
