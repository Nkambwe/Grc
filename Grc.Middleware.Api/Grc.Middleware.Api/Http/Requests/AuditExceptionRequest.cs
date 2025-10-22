using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditExceptionRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("obligation")]
        public string Obligation { get; set; }

        [JsonPropertyName("correctiveAction")]
        public string CorrectiveAction { get; set; }

        [JsonPropertyName("exceptionNoted")]
        public string ExceptionNoted { get; set; }

        [JsonPropertyName("remediationPlan")]
        public string RemediationPlan { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime TargetDate { get; set; }

        [JsonPropertyName("riskAssessment")]
        public string RiskAssessment { get; set; }

        [JsonPropertyName("riskRating")]
        public float RiskRating { get; set; }

        [JsonPropertyName("userId")]
        public string Status { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonPropertyName("auditReportId")]
        public long AuditReportId { get; set; }

        [JsonPropertyName("auditTaskId")]
        public long AuditTaskId { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
