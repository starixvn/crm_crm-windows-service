namespace CRMReportingService
{
    using System;

    using NexCel.CRM.CRMReportingService.Business.DailyReport;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.BootStrappers;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();
                BootStrapperManager.Initialize(new UnityBootStrapper());
                var dailyReportService = CommonBootStrapper.Locator.GetInstance<IDailyReportService>();
                
                #region export & send mail
                dailyReportService.ExportExcelAndSendMailToCSD();
                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
