using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class Responsebility : BaseEntity {
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPosition { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; } // IT, HR, Finance, Legal, Operations, BOD
        public virtual Department Department { get; set; }
        public virtual ICollection<AuditTask> AuditTasks { get; set; }
        public virtual ICollection<ProcessTask> ProcessTasks { get; set; }
        public virtual ICollection<RegulatoryDocument> ComplianceDocuments { get; set; }
        public virtual ICollection<OperationProcess> OwnerProcesses { get; set; }
        public virtual ICollection<OperationProcess> AssigneeProcesses { get; set; }
        public virtual ICollection<StatutoryArticle> Articles { get; set; }
        public virtual ICollection<AuditException> Findings { get; set; }
    }

}
