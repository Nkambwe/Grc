
namespace Grc.ui.App.Dtos {
    public class GrcAuditMiniReportResponse {
        public long Id { get; set; }
        public string Reference { get; set; }
        public string ReportName { get; set; }
        public string Status { get; set; }
        public DateTime AuditedOn { get; set; }
        public int Total { get; set; }
        public int Closed { get; set; }
        public int Open { get; set; }
        public int Overdue { get; set; }
        public decimal Completed { get; set; }
        public decimal Outstanding { get; set; }
        public List<GrcAuditExceptionResponse> Exceptions { get; set; } = new();
    }
}
