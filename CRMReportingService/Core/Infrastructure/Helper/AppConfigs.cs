using System;
using System.Configuration;
using DNAide.Core;

namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Helper
{
    public static class AppConfigs
    {
        public static int ReportType { get { return CastAide.AsInt32(ConfigurationSettings.AppSettings["dailysummarykey"].ToString()); } }
        public static int ReportTypeByLincensee { get { return CastAide.AsInt32(ConfigurationSettings.AppSettings["dailysummarylincennseekey"].ToString()); } }
        public static bool IsGetDateFromConfig { get { return CastAide.AsBoolean(ConfigurationSettings.AppSettings["istest"].ToString()); } }
        public static DateTime ConfigReportDate { get { return CastAide.AsDateTime(ConfigurationSettings.AppSettings["testdate"].ToString()); } }
        public static int TimeSpan { get { return CastAide.AsInt32(ConfigurationSettings.AppSettings["timeSpan"].ToString()); } }
    }
}
