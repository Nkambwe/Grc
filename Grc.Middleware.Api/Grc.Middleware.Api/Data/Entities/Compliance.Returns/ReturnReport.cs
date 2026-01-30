using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class ReturnReport: BaseEntity {
        public string ReturnName { get; set; }
        public string Risk { get; set; }
        public bool SendReminder { get; set; }
        public int SentMessages { get; set; }
        public string Interval { get; set; }
        public string IntervalType { get; set; }
        public string Reminder { get; set; }
        public string Comments { get; set; }
        public DateTime? RequiredSubmissionDate { get; set; }
        public int RequiredSubmissionDay { get; set; }
        public long OwnerId { get; set; }
        public virtual Responsebility Owner { get; set; }
        public long ArticleId { get; set; }
        public virtual StatutoryArticle Article { get; set; }
        public long AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }
        public long TypeId { get; set; }
        public virtual ReturnType ReturnType { get; set; }
        public long FrequencyId { get; set; }
        public virtual Frequency Frequency { get; set; }
        public virtual ICollection<ReturnSubmission> Submissions { get; set; }
    }

}
