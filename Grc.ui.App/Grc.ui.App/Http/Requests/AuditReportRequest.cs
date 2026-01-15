using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class AuditReportRequest {

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

        [JsonPropertyName("responseDate")]
        public DateTime? ResponseDate { get; set; }

        [JsonPropertyName("managementComments")]
        public string ManagementComments { get; set; }

        [JsonPropertyName("additionalNotes")]
        public string AdditionalNotes { get; set; }

        [JsonPropertyName("auditTypeId")]
        public string AuditTypeId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }

}
