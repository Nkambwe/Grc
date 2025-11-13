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
        public string ProcessStatus { get; set; }
        public bool? IsLockProcess { get; set; }
        public bool? NeedsBranchReview { get; set; }
        public bool? NeedsCreditReview { get; set; }
        public bool? NeedsTreasuryReview { get; set; }
        public bool? NeedsFintechReview { get; set; }
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
        public virtual ICollection<ProcessApproval> Approvals { get; set; } = new List<ProcessApproval>();
        public virtual ICollection<ProcessProcessTag> Tags { get; set; } = new List<ProcessProcessTag>();
        public ICollection<ProcessProcessGroup> Groups { get; set; } = new List<ProcessProcessGroup>();
        public virtual ICollection<ProcessActivity> Activities { get; set; }

    }

}
