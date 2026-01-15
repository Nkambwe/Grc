
namespace Grc.ui.App.Dtos {
    public class GrcAuditReportResponse {
        public long Id { get; set; }
        public string Reference { get; set; }
        public string ReportName { get; set; }
        public string Summery { get; set; }
        public string ReportStatus { get; set; }
        public DateTime ReportDate { get; set; }
        public int ExceptionCount { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ManagementComments { get; set; }
        public string AdditionalNotes { get; set; }
        public string AuditType { get; set; }
        public bool IsDeleted { get; set; }
        public List<GrcAuditUpdateResponse> ReportUpdates { get; set; } = new();
        public List<GrcAuditExceptionResponse> Exceptions { get; set; } = new();
    }
}
