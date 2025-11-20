using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ProcessApprovalStatusResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("dodStatus")]
        public string HodStatus { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

        [JsonPropertyName("complianceStatus")]
        public string ComplianceStatus { get; set; }

        [JsonPropertyName("requiresBopApproval")]
        public bool RequiresBopApproval { get; set; }

        [JsonPropertyName("complianceStatus")]
        public string BopStatus { get; set; }

        [JsonPropertyName("requiresBopApproval")]
        public bool RequiresCreditApproval { get; set; }

        [JsonPropertyName("creditStatus")]
        public string CreditStatus { get; set; }

        [JsonPropertyName("requiresTreasuryApproval")]
        public bool RequiresTreasuryApproval { get; set; }

        [JsonPropertyName("treasuryStatus")]
        public string TreasuryStatus { get; set; }

        [JsonPropertyName("requiresFintechApproval")]
        public bool RequiresFintechApproval { get; set; }

        [JsonPropertyName("fintechStatus")]
        public string FintechStatus { get; set; }
    }
}
