namespace NexCel.CRM.CRMReportingService.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class DailySummaryDTO
    {
    }

    [DataContract]
    public class DailySummaryType
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int Value { get; set; }
    }

    [DataContract]
    public class DailySummaryDetail
    {  
        [DataMember]
        public int MainCategoryId { get; set; }
        [DataMember]
        public string MainCategory { get; set; }
        [DataMember]
        public int SubCaregoryId { get; set; }
        [DataMember]
        public string Subcategory { get; set; }
        [DataMember]
        public List<DailySummaryType> Cells { get; set; }
        [DataMember]
        public int Total { get; set; }
    }

    public class DailySummaryGroup
    {
        public List<DailySummaryDetail> DailySummaries { get; set; }
        public DailySummaryDetail Total { get; set; }
    }

    [DataContract]
    public class DailySummaryBO
    {
        [DataMember]
        public DateTime ReportDay { get; set; }
        [DataMember]
        public List<DailySummaryGroup> DailySummaries { get; set; }
        [DataMember]
        public DailySummaryDetail GrandTotal { get; set; }
    }

    [DataContract]
    public class WeeklySummaryBO
    {
        [DataMember]
        public DateTime FromDay { get; set; }
        [DataMember]
        public DateTime ToDay { get; set; }
        [DataMember]
        public List<DailySummaryBO> DailyComplaints { get; set; }
        [DataMember]
        public DailySummaryBO ComplaintsStatistic { get; set; }
        [DataMember]
        public DailySummaryBO LicenseesStatistic { get; set; }
    }
}