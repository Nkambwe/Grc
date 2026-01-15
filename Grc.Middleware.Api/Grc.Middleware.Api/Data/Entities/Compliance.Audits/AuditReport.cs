namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits
{
    public class AuditReport : BaseEntity {
        public string Reference { get; set; }
        public string ReportName { get; set; }
        public string Summery { get; set; }
        public int ExceptionCount { get; set; }
        public DateTime AuditedOn { get; set; }
        public string Status { get; set; }
        public DateTime? RespondedOn { get; set; }
        public string ManagementComment { get; set; }
        public string AdditionalNotes { get; set; }
        public long AuditId { get; set; }
        public virtual Audit Audit { get; set; }
        public virtual ICollection<AuditUpdate> AuditUpdates { get; set; }
        public virtual ICollection<AuditException> AuditExceptions { get; set; }
    }
}
