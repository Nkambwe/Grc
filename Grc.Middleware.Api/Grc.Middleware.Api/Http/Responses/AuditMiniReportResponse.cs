using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class AuditMiniReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("auditedOn")]
        public DateTime AuditedOn { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("closed")]
        public int Closed { get; set; }

        [JsonPropertyName("open")]
        public int Open { get; set; }

        [JsonPropertyName("overdue")]
        public int Overdue { get; set; }

        [JsonPropertyName("completed")]
        public decimal Completed { get; set; }

        [JsonPropertyName("outstanding")]
        public decimal Outstanding { get; set; }

        [JsonPropertyName("exceptions")]
        public List<AuditExceptionResponse> Exceptions { get; set; } = new();
    }

}
