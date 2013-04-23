namespace NexCel.CRM.CRMReportingService.Entities
{
    using System;
    using System.Collections.Generic;

    public class CasinoSummaryDTO
    {
    }

    public class SystemDailySummary
    {
        public int SystemId { get; set; }
        public string Sytem { get; set; }
        public List<DailySummaryType> Cells { get; set; }
    }

    public class CasinoDailySummaryDetail
    {
        public int MainCategoryId { get; set; }
        public string MainCategory { get; set; }
        public List<SystemDailySummary> SystemSummary { get; set; }
    }

    public class CasinoDailySummary
    {
        public DateTime ReportDate { get; set; }
        public List<CasinoDailySummaryDetail> DailySummary { get; set; }
        public CasinoDailySummaryDetail GrandTotal { get; set; }
        public List<int> SumAllFeedbacks { get; set; }
    }

    public class CasinoWeeklySummary
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<CasinoDailySummary> CasinoDailySummaries { get; set; }
        public CasinoDailySummary CaisinoWeeklySummaries { get; set; }
        public DailySummaryBO WeeklySummary { get; set; }


    }
}