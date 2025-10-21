using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits {
    
    /// <summary>
    /// Class representing an audit task within the compliance audit system.
    /// </summary>
    public class AuditTask: BaseEntity {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public long AuditId { get; set; }
        public long OwnerId { get; set; }
        public virtual Responsebility ActionOwner { get; set; }
        public virtual Audit Audit { get; set; }
        public virtual ICollection<AuditException> AuditExceptions { get; set; }
    }
}
