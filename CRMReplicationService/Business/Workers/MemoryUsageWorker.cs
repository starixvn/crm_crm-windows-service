// -----------------------------------------------------------------------
// <copyright file="MemoryUsageWorker.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.CRM.CRMReplication.Core;
    using Fanex.CRM.CRMReplication.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemoryUsageWorker:BaseWorker
    {
        ISystemService systemService;
        public MemoryUsageWorker(): this(null)
        {
        }

        public MemoryUsageWorker(ISystemService service) 
        {
            systemService = service ?? new SystemService();
        }

        protected override void Run(object sender)
        {
            while (true)
            {
                if (!IsInited)
                {
                    System.Threading.Thread.Sleep(AppSettings.CheckMemoryUsageTimer);
                    IsInited = true;
                }

                try
                {
                    systemService.CheckMemoryUsage();
                }
                catch (Exception ex)
                {
                    LogExtensions.ExceptionHandler(ex);
                }
                System.Threading.Thread.Sleep(AppSettings.CheckMemoryUsageTimer);
            }
        }
    }
}
