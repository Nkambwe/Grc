
namespace Grc.ui.App.Dtos {
    public class GrcAuditResponse {
        public long Id { get; set; }
        public string Reference { get; set; }
        public string ReportName { get; set; }
        public DateTime AuditedOn { get; set; }
        public int Count { get; set; }
        public string Status { get; set; }
    }
}
