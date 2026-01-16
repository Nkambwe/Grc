namespace Grc.Middleware.Api.Http.Responses {
    public class AuditExtensionStatistics {
        public Dictionary<string, int> Statuses { get; set; }
        public List<AuditMiniReportResponse> Reports { get; set; }
    }

}
