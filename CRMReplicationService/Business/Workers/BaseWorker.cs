// -----------------------------------------------------------------------
// <copyright file="BaseWorker.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business.Workers
{
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseWorker
    {
        protected bool IsInited;
        protected Thread Thread;

        public BaseWorker()
        {
            Thread = new Thread(Run);
        }

        protected virtual void Run(object sender)
        {
        }

        public void Start()
        {
            Thread.Start();
        }

        public void Stop()
        {
            Thread.Abort();
        }

    }
}
