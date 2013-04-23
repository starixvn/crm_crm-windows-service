// -----------------------------------------------------------------------
// <copyright file="CustomerWorker.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business.Workers
{
    using System;

    using Fanex.CRM.CRMReplication.Core;
    using Fanex.CRM.CRMReplication.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerWorker: BaseWorker
    {
        private ICustomerService customerService;

        public CustomerWorker()
            : this(null)
        {
        }

        public CustomerWorker(ICustomerService service)
        {
            customerService = service ?? new CustomerService();
        }

        protected override void Run(object sender)
        {
            while (true)
            {
                if (!IsInited)
                {
                    System.Threading.Thread.Sleep(AppSettings.CustomerTimer);
                    IsInited = true;
                }

                try
                {
                    customerService.ReplicateData();
                }
                catch (Exception ex)
                {
                    LogExtensions.ExceptionHandler(ex);
                }
                System.Threading.Thread.Sleep(AppSettings.CustomerTimer);
            }
        }
    }
}
