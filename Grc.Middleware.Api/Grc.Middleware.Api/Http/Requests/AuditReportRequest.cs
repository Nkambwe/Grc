using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditReportRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("exceptionCount")]
        public int ExceptionCount { get; set; }

        [JsonPropertyName("auditedOn")]
        public DateTime AuditedOn { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("respondedOn")]
        public DateTime? RespondedOn { get; set; }

        [JsonPropertyName("managementComment")]
        public string ManagementComment { get; set; }

        [JsonPropertyName("additionalNotes")]
        public string AdditionalNotes { get; set; }

        [JsonPropertyName("auditId")]
        public long AuditId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }

}
