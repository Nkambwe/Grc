using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Regulations {
    public class RegulatoryDocument : BaseEntity {
        public string DocumentName { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public bool PolicyAligned { get; set; }
        public bool? IsLocked { get; set; }
        public bool NeedMcrApproval { get; set; }
        public bool NeedBoardApproval { get; set; }
        public long FrequencyId { get; set; }
        public DateTime LastRevisionDate { get; set; }
        public DateTime? NextRevisionDate { get; set; }
        public bool SendNotification { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public int SentMessages { get; set; }
        public string NextSendAt {  get; set; }
        public string ReminderMessage { get; set; }
        public long DocumentTypeId { get; set; }
        public long ResponsibilityId { get; set; }
        public string Comments { get; set; }
        public bool OnIntranet { get; set; }
        public bool IsApproved { get; set; }
        public virtual Responsebility Owner { get; set; }
        public virtual Frequency Frequency { get; set; }
        public virtual RegulatoryDocumentType DocumentType { get; set; }
    }

}
