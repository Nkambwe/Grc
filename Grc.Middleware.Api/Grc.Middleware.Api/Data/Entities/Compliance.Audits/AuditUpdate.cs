namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits
{
    public class AuditUpdate : BaseEntity { 
        public string Notes { get; set; }
        public DateTime NoteDate { get; set; }
        public bool SendReminder { get; set; }
        public DateTime? SendReminderOn { get; set; }
        public string ReminderMessage { get; set; }
        public string SendTo { get; set; }
        public string AddedBy { get; set; }
        public long ReportId { get; set; }
        public virtual AuditReport Report { get; set; }
    }
}
