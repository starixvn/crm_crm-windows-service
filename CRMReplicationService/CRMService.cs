namespace Fanex.CRM.CRMReplication
{
    using System.ServiceProcess;
    using Fanex.CRM.CRMReplication.Business.Workers;
    using Fanex.CRM.CRMReplication.Core.Helper;
    using Fanex.CRM.CRMReplication.Core;

    partial class CRMService : ServiceBase
    {
        private readonly CustomerWorker cusomerWorker;
        //private readonly MemoryUsageWorker checkMemoryUsagedWorker;
        public CRMService()
        {
            InitializeComponent();
            this.cusomerWorker = new CustomerWorker();
            //this.checkMemoryUsagedWorker = new MemoryUsageWorker();
        }

        protected override void OnStart(string[] args)
        {
            Data.Configuration.DbSettingProviderManager.Current.Start();
            //CacheService.Set(Constant.CacheKeys.MaxTsTmp, "0000000000000001");
            
            this.cusomerWorker.Start();
            //this.checkMemoryUsagedWorker.Start();
            Logger.WriteLog("Start service");
        }

        protected override void OnStop()
        {
            var maxTime = Fanex.CRM.CRMReplication.Core.CacheService.Get(Fanex.CRM.CRMReplication.Core.Constant.CacheKeys.MaxTsTmp) as string;
            if(!string.IsNullOrEmpty(maxTime))
                Fanex.CRM.CRMReplication.Core.AppSettings.CustomerMaxTsTamp = maxTime;

            Logger.WriteLog("Stop service");
            this.cusomerWorker.Stop();
            //this.checkMemoryUsagedWorker.Stop();
        }
    }
}
