namespace NexCel.CRM.CRMReportingService.Entities
{
    using System.Collections.Generic;

    public class DailyLicenseeDTO
    {
        public List<DailyLicenseeDetail> LicenseeComplaints { get; set; }
    }

    public class DailyLicenseeDetail
    {
        public string FeedbackDate { get; set; }
        public string Licensee { get; set; }
        public string CustomerName { get; set; }
        public string ContactAddress { get; set; }
        public string Category { get; set; }
        public string Complaints { get; set; }
        public string Solution { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public string HandlyBy { get; set; }
        public string DutyShift { get; set; }
    }
}
