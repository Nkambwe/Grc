using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcProcessApprovalStatusResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processDescription")]
        public string ProcessDescription { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("headOfDepartmentStatus")]
        public string HodStatus { get; set; }

        [JsonPropertyName("headOfDepartmentEnd")]
        public DateTime? HodEnd { get; set; }

        [JsonPropertyName("headOfDepartmentComment")]
        public string HodComment { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

        [JsonPropertyName("riskStart")]
        public DateTime? RiskStart { get; set; }

        [JsonPropertyName("riskEnd")]
        public DateTime? RiskEnd { get; set; }

        [JsonPropertyName("riskComment")]
        public string RiskComment { get; set; }

        [JsonPropertyName("complianceStart")]
        public DateTime? ComplianceStart { get; set; }

        [JsonPropertyName("complianceEnd")]
        public DateTime? ComplianceEnd { get; set; }

        [JsonPropertyName("complianceStatus")]
        public string ComplianceStatus { get; set; }

        [JsonPropertyName("complianceComment")]
        public string ComplianceComment { get; set; }

        [JsonPropertyName("requiresBopApproval")]
        public bool RequiresBopApproval { get; set; }

        [JsonPropertyName("branchOperationsStatus")]
        public string BopStatus { get; set; }

        [JsonPropertyName("branchOperationsStatusStart")]
        public DateTime? BopStart { get; set; }

        [JsonPropertyName("branchOperationsStatusEnd")]
        public DateTime? BopEnd { get; set; }

        [JsonPropertyName("branchManagerComment")]
        public string BopComment { get; set; }

        [JsonPropertyName("requiresCreditApproval")]
        public bool RequiresCreditApproval { get; set; }

        [JsonPropertyName("creditStatus")]
        public string CreditStatus { get; set; }

        [JsonPropertyName("creditStart")]
        public DateTime? CreditStart { get; set; }

        [JsonPropertyName("creditEnd")]
        public DateTime? CreditEnd { get; set; }

        [JsonPropertyName("creditComment")]
        public string CreditComment { get; set; }

        [JsonPropertyName("requiresTreasuryApproval")]
        public bool RequiresTreasuryApproval { get; set; }

        [JsonPropertyName("treasuryStatus")]
        public string TreasuryStatus { get; set; }

        [JsonPropertyName("treasuryStart")]
        public DateTime? TreasuryStart { get; set; }

        [JsonPropertyName("treasuryEnd")]
        public DateTime? TreasuryEnd { get; set; }

        [JsonPropertyName("treasuryComment")]
        public string TreasuryComment { get; set; }

        [JsonPropertyName("requiresFintechApproval")]
        public bool RequiresFintechApproval { get; set; }

        [JsonPropertyName("fintechStatus")]
        public string FintechStatus { get; set; }

        [JsonPropertyName("fintechStart")]
        public DateTime? FintechStart { get; set; }

        [JsonPropertyName("fintechEnd")]
        public DateTime? FintechEnd { get; set; }

        [JsonPropertyName("fintechComment")]
        public string FintechComment { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
