namespace NexCel.CRM.CRMReportingService.Entities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class DailyComplaintsDataDTO
    {
        public List<DailyComplaintsDetail> DailyComplaints { get; set; }
    }

    [DataContract]
    public class CommunicationTool
    {
        [DataMember]
        public string ContactBy { get; set; }
        [DataMember]
        public string Address { get; set; }
    }

    [DataContract]
    public class DailyComplaintsDetail
    {
        [DataMember]
        public string FeedbackDate { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public CommunicationTool CommunicationTool { get; set; }
        [DataMember]
        public string ExtNo { get; set; }
        public string ProductName { get; set; }
        public string Licensee { get; set; }
        [DataMember]
        public string Site { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public string Complaints { get; set; }
        [DataMember]
        public string Solution { get; set; }
        [DataMember]
        public string Response { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string HandlyBy { get; set; }
        [DataMember]
        public string DutyShift { get; set; }
    }
}
