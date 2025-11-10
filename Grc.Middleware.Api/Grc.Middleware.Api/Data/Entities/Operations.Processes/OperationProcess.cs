using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    
    public class OperationProcess: BaseEntity {
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public string CurrentVersion { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string FileName { get; set; }
        public bool OriginalOnFile { get; set; }
        public string ProcessStatus { get; set; }
        public bool IsLockProcess { get; set; }
        public bool NeedsBranchReview { get; set; }
        public bool NeedsCreditReview { get; set; }
        public bool NeedsTreasuryReview { get; set; }
        public bool NeedsFintechReview { get; set; }
        public string Comments { get; set; }
        public string ReasonOnhold { get; set; }
        public long UnitId { get; set; }
        public long ResponsibilityId { get; set; }
        public long OwnerId { get; set; }
        public long TypeId { get; set; }
        public virtual ProcessType ProcessType { get; set; }
        public virtual DepartmentUnit Unit { get; set; }
        public virtual Responsebility Responsible { get; set; }
        public virtual Responsebility Owner { get; set; }
        public virtual ICollection<ProcessTask> Tasks { get; set; }
        public virtual ICollection<ProcessApprovals> Approvals { get; set; } = new List<ProcessApprovals>();
        public virtual ICollection<ProcessProcessTag> Tags { get; set; } = new List<ProcessProcessTag>();
        public ICollection<ProcessProcessGroup> Groups { get; set; } = new List<ProcessProcessGroup>();
        public virtual ICollection<ProcessActivity> Activities { get; set; }

    }

    public class ProcessApprovals: BaseEntity {
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
