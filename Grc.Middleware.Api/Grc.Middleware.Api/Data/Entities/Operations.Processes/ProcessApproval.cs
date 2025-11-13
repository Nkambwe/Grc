namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessApproval: BaseEntity {
        public DateTime? HeadOfDepartmentStart { get; set; }
        public DateTime? HeadOfDepartmentEnd { get; set; }
        public string HeadOfDepartmentStatus { get; set; }
        public string HeadOfDepartmentComment { get; set; }
        public DateTime? RiskStart { get; set; }
        public DateTime? RiskEnd { get; set; }
        public string RiskStatus { get; set; }
        public string RiskComment { get; set; }
        public DateTime? ComplianceStart { get; set; }
        public DateTime? ComplianceEnd { get; set; }
        public string ComplianceStatus { get; set; }
        public string ComplianceComment { get; set; }
        public DateTime? BranchOperationsStatusStart { get; set; }
        public DateTime? BranchOperationsStatusEnd { get; set; }
        public string BranchOperationsStatus { get; set; }
        public string BranchManagerComment { get; set; }
        public DateTime? CreditStart { get; set; }
        public DateTime? CreditEnd { get; set; }
        public string CreditStatus { get; set; }
        public string CreditComment { get; set; }
        public DateTime? TreasuryStart { get; set; }
        public DateTime? TreasuryEnd { get; set; }
        public string TreasuryStatus { get; set; }
        public string TreasuryComment { get; set; }
        public DateTime? FintechStart { get; set; }
        public DateTime? FintechEnd { get; set; }
        public string FintechStatus { get; set; }
        public string FintechComment { get; set; }
        public long ProcessId { get; set; }
        public virtual OperationProcess Process { get; set; }
    }

}
