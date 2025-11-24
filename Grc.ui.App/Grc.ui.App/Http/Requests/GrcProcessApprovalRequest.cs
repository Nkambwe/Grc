using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcProcessApprovalRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("hodStatus")]
        public string HodStatus { get; set; }

        [JsonPropertyName("hodComment")]
        public string HodComment { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

        [JsonPropertyName("riskComment")]
        public string RiskComment { get; set; }

        [JsonPropertyName("complianceStatus")]
        public string ComplianceStatus { get; set; }

        [JsonPropertyName("complianceComment")]
        public string ComplianceComment { get; set; }

        [JsonPropertyName("bopRequired")]
        public bool BopRequired { get; set; }

        [JsonPropertyName("bopStatus")]
        public string BopStatus { get; set; }

        [JsonPropertyName("bopComment")]
        public string BopComment { get; set; }

        [JsonPropertyName("creditRequired")]
        public bool CreditRequired { get; set; }

        [JsonPropertyName("creditStatus")]
        public string CreditStatus { get; set; }

        [JsonPropertyName("creditComment")]
        public string CreditComment { get; set; }

        [JsonPropertyName("treasuryRequired")]
        public bool TreasuryRequired { get; set; }

        [JsonPropertyName("treasuryStatus")]
        public string TreasuryStatus { get; set; }

        [JsonPropertyName("treasuryComment")]
        public string TreasuryComment { get; set; }

        [JsonPropertyName("fintechRequired")]
        public bool FintechRequired { get; set; }

        [JsonPropertyName("fintechStatus")]
        public string FintechStatus { get; set; }

        [JsonPropertyName("fintechComment")]
        public string FintechComment { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
