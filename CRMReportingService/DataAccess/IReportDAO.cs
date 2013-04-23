namespace NexCel.CRM.CRMReportingService.DataAccess
{
    using System;
    using System.Data;

    using NexCel.CRM.CRMReportingService.Core.Constant;

    public interface IReportDAO
    {
        DataSet GetDailySummaryReport(int type, DateTime fromDate, DateTime toDate, Enumerations.ReportTypes siteType);
        DataSet GetLicenseeSummaryReport(DateTime fromDate, DateTime toDate);

        DataSet GetDailyComplaintsReport(DateTime fromDate, DateTime toDate, Enumerations.ReportTypes siteType);

        DataSet GetDailyLicenseeReport(DateTime fromDate, DateTime toDate);

        DataTable GetSettings();

        DataTable SelCountries();

        DataSet GetCasinoSummaryDailyReport(int reportType, DateTime fromDate, DateTime toDate);

        DataSet GetCasinoDailyDetailReport(int reportType, DateTime fromDate, DateTime toDate);
    }
}
