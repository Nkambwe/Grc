using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ProcessApprovalResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("headOfDepartmentStart")]
        public DateTime? HeadOfDepartmentStart { get; set; }

        [JsonPropertyName("headOfDepartmentEnd")]
        public DateTime? HeadOfDepartmentEnd { get; set; }

        [JsonPropertyName("headOfDepartmentStatus")]
        public string HeadOfDepartmentStatus { get; set; }

        [JsonPropertyName("headOfDepartmentComment")]
        public string HeadOfDepartmentComment { get; set; }

        [JsonPropertyName("riskStart")]
        public DateTime? RiskStart { get; set; }

        [JsonPropertyName("riskEnd")]
        public DateTime? RiskEnd { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

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

        [JsonPropertyName("branchOperationsStatusStart")]
        public DateTime? BranchOperationsStatusStart { get; set; }

        [JsonPropertyName("branchOperationsStatusEnd")]
        public DateTime? BranchOperationsStatusEnd { get; set; }

        [JsonPropertyName("branchOperationsStatus")]
        public string BranchOperationsStatus { get; set; }

        [JsonPropertyName("branchManagerComment")]
        public string BranchManagerComment { get; set; }

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

        [JsonPropertyName("tintechStatus")]
        public string FintechStatus { get; set; }

        [JsonPropertyName("fintechComment")]
        public string FintechComment { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
