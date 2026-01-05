using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class Circular: BaseEntity {       
        public string CircularTitle { get; set; }
        public DateTime RecievedOn { get; set; }
        public DateTime DeadlineOn { get; set; }
        public string Status { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string FilePath { get; set; }
        public string SubmittedBy { get; set; }
        public string RefNumber { get; set; }
        public string Comments { get; set; }
        public long AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }
        public long FrequencyId { get; set; }
        public virtual Frequency Frequency { get; set; }
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<CircularIssue> Issues { get; set; }
    }
}
