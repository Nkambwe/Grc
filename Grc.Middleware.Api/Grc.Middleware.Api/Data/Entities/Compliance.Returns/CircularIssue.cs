namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class CircularIssue : BaseEntity {
        public string IssueDescription { get; set; }
        public string Resolution { get; set; }
        public string Status { get; set; }
        public DateTime RecievedOn { get; set; }
        public DateTime? ResolvedOn { get; set; }
        public long CircularId { get; set; }
        public virtual Circular Circular { get; set; }
        
    }
}
