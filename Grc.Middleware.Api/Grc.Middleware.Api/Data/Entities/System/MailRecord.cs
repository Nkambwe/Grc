using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.System {
    public class MailRecord : BaseEntity {
        public long? DocumentId { get; set; }
        public long? ReturnId { get; set; }
        public long? SubmissionId { get; set; }
        public long? CircularId { get; set; }
        public string SentToEmail { get; set; }
        public string CCMail { get; set; }
        public string Subject { get; set; }
        public string Mail {  get; set; }
        public long? ApprovalId { get; set; }
        public virtual ProcessApproval Approval { get; set; }
    }
}
