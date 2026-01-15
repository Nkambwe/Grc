namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits {
    public class AuditType : BaseEntity {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Audit> Audits { get; set; }
    }
}
