using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcProcessTatResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("hodStartdate")]
        public DateTime? HodStartdate { get; set; }

        [JsonPropertyName("hodEnddate")]
        public DateTime? HodEnddate { get; set; }

        [JsonPropertyName("hodStatus")]
        public string HodStatus { get; set; }

        [JsonPropertyName("hodComment")]
        public string HodComment { get; set; }

        [JsonPropertyName("hodCount")]
        public int HodCount { get; set; }

        [JsonPropertyName("riskStartdate")]
        public DateTime? RiskStartdate { get; set; }

        [JsonPropertyName("riskEnddate")]
        public DateTime? RiskEnddate { get; set; }

        [JsonPropertyName("riskStatus")]
        public string RiskStatus { get; set; }

        [JsonPropertyName("riskComment")]
        public string RiskComment { get; set; }

        [JsonPropertyName("riskCount")]
        public int RiskCount { get; set; }

        [JsonPropertyName("complianceStartdate")]
        public DateTime? ComplianceStartdate { get; set; }

        [JsonPropertyName("complianceEnddate")]
        public DateTime? ComplianceEnddate { get; set; }

        [JsonPropertyName("complianceStatus")]
        public string ComplianceStatus { get; set; }

        [JsonPropertyName("complianceComment")]
        public string ComplianceComment { get; set; }

        [JsonPropertyName("complianceCount")]
        public int ComplianceCount { get; set; }

        [JsonPropertyName("needBropsApproval")]
        public bool NeedBropsApproval { get; set; }

        [JsonPropertyName("bropStartdate")]
        public DateTime? BropStartdate { get; set; }

        [JsonPropertyName("bropEnddate")]
        public DateTime? BropEnddate { get; set; }

        [JsonPropertyName("bropStatus")]
        public string BropStatus { get; set; }

        [JsonPropertyName("bropComment")]
        public string BropComment { get; set; }

        [JsonPropertyName("bopCount")]
        public int BopCount { get; set; }

        [JsonPropertyName("needTreasuryApproval")]
        public bool NeedTreasuryApproval { get; set; }

        [JsonPropertyName("treasuryStartdate")]
        public DateTime? TreasuryStartdate { get; set; }

        [JsonPropertyName("treasuryEnddate")]
        public DateTime? TreasuryEnddate { get; set; }

        [JsonPropertyName("treasuryStatus")]
        public string TreasuryStatus { get; set; }

        [JsonPropertyName("treasuryComment")]
        public string TreasuryComment { get; set; }

        [JsonPropertyName("treasuryCount")]
        public int TreasuryCount { get; set; }

        [JsonPropertyName("needFintechApproval")]
        public bool NeedFintechApproval { get; set; }

        [JsonPropertyName("fintechStartdate")]
        public DateTime? FintechStartdate { get; set; }

        [JsonPropertyName("fintechEnddate")]
        public DateTime? FintechEnddate { get; set; }

        [JsonPropertyName("fintechStatus")]
        public string FintechStatus { get; set; }

        [JsonPropertyName("fintechComment")]
        public string FintechComment { get; set; }

        [JsonPropertyName("fintechCount")]
        public int FintechCount { get; set; }

        [JsonPropertyName("needCreditApproval")]
        public bool NeedCreditApproval { get; set; }

        [JsonPropertyName("creditStartdate")]
        public DateTime? CreditStartdate { get; set; }

        [JsonPropertyName("creditEnddate")]
        public DateTime? CreditEnddate { get; set; }

        [JsonPropertyName("creditStatus")]
        public string CreditStatus { get; set; }

        [JsonPropertyName("creditComment")]
        public string CreditComment { get; set; }

        [JsonPropertyName("creditCount")]
        public int CreditCount { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

    }

}
