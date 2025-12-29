namespace Grc.Middleware.Api.Data.Entities.Compliance.Regulations {
    public class ComplianceIssue : BaseEntity {
        public string Description { get; set; }
        public string Notes { get; set; }
        public long StatutoryArticleId { get; set; }
        public virtual StatutoryArticle StatutoryArticle { get; set; }
    }

}
