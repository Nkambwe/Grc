namespace Grc.ui.App.Dtos {
    public class AuditExtensionStatistics {
        public Dictionary<string, int> Statuses { get; set; }
        public List<GrcAuditMiniReportResponse> Reports { get; set; }
    }
}
