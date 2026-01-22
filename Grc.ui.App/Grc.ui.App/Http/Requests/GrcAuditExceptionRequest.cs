using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcAuditExceptionRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportId")]
        public long ReportId { get; set; }

        [JsonPropertyName("findings")]
        public string Findings { get; set; }

        [JsonPropertyName("recomendations")]
        public string Recomendations { get; set; }

        [JsonPropertyName("proposedAction")]
        public string ProposedAction { get; set; }

        [JsonPropertyName("correctiveAction")]
        public string CorrectiveAction { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("responsibileId")]
        public long ResponsibileId { get; set; }

        [JsonPropertyName("executioner")]
        public string Executioner { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime TargetDate { get; set; }

        [JsonPropertyName("riskLevel")]
        public string RiskLevel { get; set; }

        [JsonPropertyName("riskRate")]
        public decimal RiskRate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

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
