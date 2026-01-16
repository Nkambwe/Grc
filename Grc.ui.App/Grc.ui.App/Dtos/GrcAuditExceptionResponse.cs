
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {
    public class GrcAuditExceptionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportId")]
        public long ReportId { get; set; }

        [JsonPropertyName("finding")]
        public string Finding { get; set; }

        [JsonPropertyName("proposedAction")]
        public string ProposedAction { get; set; }

        [JsonPropertyName("correctiveAction")]
        public string CorrectiveAction { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("targetDate")]
        public DateTime? TargetDate { get; set; }

        [JsonPropertyName("riskStatement")]
        public string RiskStatement { get; set; }

        [JsonPropertyName("riskRating")]
        public decimal RiskRating { get; set; }

        [JsonPropertyName("responsibleId")]
        public long ResponsibleId { get; set; }

        [JsonPropertyName("responsible")]
        public string Responsible { get; set; }

        [JsonPropertyName("excutioner")]
        public string Executioner { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("tasks")]
        public List<GrcAuditTaskResponse> Tasks { get; set; } = new();

    }

}
