using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    
    public class OperationProcess: BaseEntity {
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public string CurrentVersion { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string FilePath { get; set; }
        public bool OriginalOnFile { get; set; }
        public string ProcessStatus { get; set; }
        /// <summary>
        /// Draft, Approved, Rejected, On Hold, Request[User sent approval status but not yet attended to]
        /// </summary>
        public string ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public string Comments { get; set; }
        public string ReasonOnhold { get; set; }
        public long UnitId { get; set; }
        public long ResponsibilityId { get; set; }
        public long TypeId { get; set; }
        public virtual ProcessType ProcessType { get; set; }
        public virtual DepartmentUnit Unit { get; set; }
        public virtual Responsebility Owner { get; set; }
        public virtual ICollection<ProcessTask> Tasks { get; set; }
        public virtual ICollection<ProcessProcessTag> Tags { get; set; }
        public virtual ICollection<ProcessProcessGroup> Groups { get; set; }
        public virtual ICollection<ProcessActivity> Activities { get; set; }

    }

}
