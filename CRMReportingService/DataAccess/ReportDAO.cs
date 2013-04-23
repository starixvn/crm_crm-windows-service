namespace NexCel.CRM.CRMReportingService.DataAccess
{
    using System;
    using System.Collections;
    using System.Data;

    using Fanex.Data;

    using NexCel.CRM.CRMReportingService.Core.Constant;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations;
    using NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper;

    public class ReportDAO : BaseDataAccess, IReportDAO
    {
        protected StoreProcedureElement GetStoreProcedure(string spAlias)
        {
            return GetStoreProcedure(StoreProcedureSection.StoreCollection.Report, spAlias);
        }

        public DataSet GetDailySummaryReport(int type, DateTime fromDate, DateTime toDate, Enumerations.ReportTypes siteType)
        {
            DataSet result = null;
            try
            {
                var param = new Hashtable
                                {
                                    {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)},
                                    {"sitetype", (int) siteType}
                                   
                                };

                using (var objDB = new ObjectDb("dailysummaryreport"))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();
                    
                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }
            
            return result;
        }

        public DataSet GetLicenseeSummaryReport(DateTime fromDate, DateTime toDate)
        {
            var result = new DataSet();
            try
            {
                var param = new Hashtable
                                {
                                     {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)}
                                };

                using (var objDB = new ObjectDb("dailylicenseesummaryreport", 1000))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();

                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }

        public DataSet GetDailyComplaintsReport(DateTime fromDate, DateTime toDate, Enumerations.ReportTypes siteType)
        {
            var result = new DataSet();
            try
            {
                var param = new Hashtable
                                {
                                    {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)},
                                    {"sitetype", (int) siteType}
                                };

                using (var objDB = new ObjectDb("dailydetailreport", 1000))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();

                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }

        public DataSet GetDailyLicenseeReport(DateTime fromDate, DateTime toDate)
        {
            var result = new DataSet();
            try
            {
                var param = new Hashtable {
                                     {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)}
                };

                using (var objDB = new ObjectDb("dailylicenseedetailreport", 1000))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();

                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }

        public DataTable GetSettings()
        {
            var result = new DataTable();
            try
            {

                using (var objDB = new ObjectDb("getsettings"))
                {
                    var ds = objDB.ExecuteDataSet();
                    if (ds != null && ds.Tables.Count > 0)
                        result = ds.Tables[0];
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }
        
        public DataSet GetCasinoSummaryDailyReport(int reportType, DateTime fromDate, DateTime toDate)
        {
            var result = new DataSet();
            try
            {
                var param = new Hashtable
                                {
                                    {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)},
                                    {"type", reportType}  
                                };

                using (var objDB = new ObjectDb("casinodailysummary", 1000))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();

                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }
        
        public DataSet GetCasinoDailyDetailReport(int reportType, DateTime fromDate, DateTime toDate)
        {
            var result = new DataSet();
            try
            {
                var param = new Hashtable
                                {
                                    //{"type", reportType},
                                    {"fromdate", DateTimeUtilize.ChangeTimeToServerForReportBegin(fromDate)},
                                    {"todate", DateTimeUtilize.ChangeTimeToServerForReportEnd(toDate)}
                                };

                using (var objDB = new ObjectDb("casinodailydetail", 1000))
                {
                    objDB.AssignParamValues(param);
                    result = objDB.ExecuteDataSet();

                    return result;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }

        public DataTable SelCountries()
        {
            var result = new DataTable();
            try
            {

                using (var objDB = new ObjectDb("selCountries"))
                {
                    var ds = objDB.ExecuteDataSet();
                    if (ds != null && ds.Tables.Count > 0)
                        result = ds.Tables[0];
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); }

            return result;
        }
    }
}
