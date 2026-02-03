
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {

    public class GrcAuditReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("reportStatus")]
        public string ReportStatus { get; set; }

        [JsonPropertyName("reportDate")]
        public DateTime ReportDate { get; set; }

        [JsonPropertyName("exceptionCount")]
        public int ExceptionCount { get; set; }

        [JsonPropertyName("resolvedException")]
        public int ResolvedException { get; set; }

        [JsonPropertyName("responseDate")]
        public DateTime? ResponseDate { get; set; }

        [JsonPropertyName("managementComments")]
        public string ManagementComments { get; set; }

        [JsonPropertyName("additionalNotes")]
        public string AdditionalNotes { get; set; }

        [JsonPropertyName("auditId")]
        public long AuditId { get; set; }

        [JsonPropertyName("auditType")]
        public string AuditType { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("updates")]
        public List<GrcAuditUpdateResponse> Updates { get; set; } = new();

        [JsonPropertyName("findings")]
        public List<GrcAuditExceptionResponse> Findings { get; set; } = new();
    }
}
