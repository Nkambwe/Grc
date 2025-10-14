namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class StatutoryRegulation: BaseEntity {
        public string Code { get; set; }
        public string RegulatoryName { get; set; }
        public long TypeId { get; set; }
        public long AuthorityId { get; set; }
        public long CategoryId { get; set; }
        public virtual Authority Authority { get; set; }
        public virtual RegulatoryType RegulationType { get; set; }
        public virtual RegulatoryCategory Category { get; set; }
        public virtual ICollection<StatutoryArticle> Articles { get; set; }
    }
}
