namespace NexCel.CRM.CRMReportingService.DataAccess
{
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations;

    public class BaseDataAccess
    {
        protected virtual StoreProcedureElement GetStoreProcedure(StoreProcedureSection.StoreCollection storeCollection, string aliasName)
        {
            var sps = StoreProcedureSection.Instance[storeCollection];
            return sps != null ? sps[aliasName] : null;
        }
    }
}
