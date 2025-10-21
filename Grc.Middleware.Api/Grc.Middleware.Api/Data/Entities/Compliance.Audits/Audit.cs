using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits {
    public class Audit: BaseEntity {
        public string AuditName { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int NumberOfReports { get; set; }
        public string Attachement { get; set; }
        public long AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }
        public virtual ICollection<AuditTask> Tasks { get; set; }
        public virtual ICollection<AuditReport> AuditReports { get; set; }
    }
}
