using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns {
    public class RegulatoryReturn: BaseEntity {
        public string ReturnName { get; set; }
        public string Description { get; set; }
        public string RequirementDetails { get; set; }
        public long TypeId { get; set; }
        public long FrequencyId { get; set; }
        public string FrequencyInfo { get; set; }
        public DateTime? LastSubmissionDate { get; set; }
        /// <summary>
        /// Get or Set the next submission date for the regulatory return
        /// </summary>
        /// <remarks>If Submission report not found create record</remarks>
        public DateTime? NextSubmissionDate { get; set; }
        public long ArticleId { get; set; }
        public long AuthorityId { get; set; }
        public long ResponsibilityId { get; set; }
        /// <summary>
        /// Checks whether the obligation is currently compliant or not or Not Applicable
        /// </summary>
        public string ComplianceGap { get; set; }
        public string Comments { get; set; }
        /// <summary>
        /// Gets or sets the responsibility associated with this entity.
        /// </summary>
        public virtual Responsebility Responsibility { get; set; }
        public virtual StatutoryArticle Article { get; set; }
        public virtual ReturnType ReturnType { get; set; }
        public virtual Frequency Frequency { get; set; }
        public virtual ICollection<ReturnSubmission> Submissions { get; set; }
    }

}
