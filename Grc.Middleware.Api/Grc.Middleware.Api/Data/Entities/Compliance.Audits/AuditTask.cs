using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits {
    
    /// <summary>
    /// Class representing an audit task within the compliance audit system.
    /// </summary>
    public class AuditTask: BaseEntity {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public bool SendReminder { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string Reminder { get; set; }
        public long OwnerId { get; set; }
        public virtual Responsebility ActionOwner { get; set; }
        public long ExceptionId { get; set; }
        public virtual AuditException AuditException { get; set; }
    }
}
