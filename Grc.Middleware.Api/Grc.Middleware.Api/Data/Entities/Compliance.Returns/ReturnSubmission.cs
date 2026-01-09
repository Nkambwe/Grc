namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class ReturnSubmission: BaseEntity {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Status { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string FilePath { get; set; }
        public string SubmittedBy { get; set; }
        public string Comments { get; set; }
        public long ReturnId { get; set; }
        public virtual ReturnReport RegulatoryReturn { get; set; }
        public virtual ICollection<SubmissionNotification> Notifications { get; set; }
    }
}
