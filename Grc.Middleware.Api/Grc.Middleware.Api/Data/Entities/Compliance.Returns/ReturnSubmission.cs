namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class ReturnSubmission: BaseEntity {
        public string RefNumber { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }
        public string SubmittedBy { get; set; }
        public string ResponseRef { get; set; }
        public string Comments { get; set; }
        public long ReturnId { get; set; }
        public virtual RegulatoryReturn RegulatoryReturn { get; set; }
        public virtual ICollection<SubmissionNotification> Notifications { get; set; }
    }
}
