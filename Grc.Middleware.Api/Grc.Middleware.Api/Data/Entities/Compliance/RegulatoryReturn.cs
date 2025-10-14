namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class RegulatoryReturn: BaseEntity {
        public string Code { get; set; }
        public string ReturnName { get; set; }
        public string Description { get; set; }
        public long TypeId { get; set; }
        public long FrequencyId { get; set; }
        public string FrequencyInfo { get; set; }
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
        public virtual Responsibility Responsibility { get; set; }
        public virtual StatutoryArticle Article { get; set; }
        public virtual ReturnType ReturnType { get; set; }
        public virtual Frequency Frequency { get; set; }
    }

}
