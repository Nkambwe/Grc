using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class ProcessApprovalRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("hodStart")]
        public DateTime? HODStart { get; set; }

        [JsonPropertyName("hodEnd")]
        public DateTime? HODEnd { get; set; }

        [JsonPropertyName("hodStatus")]
        public string HODStatus { get; set; }

        [JsonPropertyName("hodComment")]
        public string HODComment { get; set; }

        [JsonPropertyName("riskStart")]
        public DateTime? RiskStart { get; set; }

        [JsonPropertyName("riskEnd")]
        public DateTime? RiskEnd { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

        [JsonPropertyName("riskComment")]
        public string RiskComment { get; set; }

        [JsonPropertyName("compStart")]
        public DateTime? ComplianceStart { get; set; }

        [JsonPropertyName("compEnd")]
        public DateTime? ComplianceEnd { get; set; }

        [JsonPropertyName("compStatus")]
        public string ComplianceStatus { get; set; }

        [JsonPropertyName("compComment")]
        public string ComplianceComment { get; set; }

        [JsonPropertyName("bopStart")]
        public DateTime? BOPStart { get; set; }

        [JsonPropertyName("bopEnd")]
        public DateTime? BOPEnd { get; set; }

        [JsonPropertyName("bopStatus")]
        public string BOPStatus { get; set; }

        [JsonPropertyName("bopComment")]
        public string BOPComment { get; set; }

        [JsonPropertyName("creditStart")]
        public DateTime? CreditStart { get; set; }

        [JsonPropertyName("creditEnd")]
        public DateTime? CreditEnd { get; set; }

        [JsonPropertyName("creditStatus")]
        public string CreditStatus { get; set; }

        [JsonPropertyName("creditComment")]
        public string CreditComment { get; set; }

        [JsonPropertyName("treasuryStart")]
        public DateTime? TreasuryStart { get; set; }

        [JsonPropertyName("treasuryEnd")]
        public DateTime? TreasuryEnd { get; set; }

        [JsonPropertyName("treasuryStatus")]
        public string TreasuryStatus { get; set; }

        [JsonPropertyName("treasuryComment")]
        public string TreasuryComment { get; set; }

        [JsonPropertyName("fintechStart")]
        public DateTime? FintechStart { get; set; }

        [JsonPropertyName("fintechEnd")]
        public DateTime? FintechEnd { get; set; }

        [JsonPropertyName("fintechStatus")]
        public string FintechStatus { get; set; }

        [JsonPropertyName("fintechComment")]
        public string FintechComment { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }

}
