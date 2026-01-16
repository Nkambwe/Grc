using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits {

    public class Audit: BaseEntity {
        public string AuditName { get; set; }
        public string Notes { get; set; }
        public long AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }
        public long AuditTypeId { get; set; }
        public virtual AuditType AuditType { get; set; }
        public virtual ICollection<AuditReport> AuditReports { get; set; }
       
    }

}
