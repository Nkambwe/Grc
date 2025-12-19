using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.Support;

public class StatutoryArticle : BaseEntity {
    public string Article { get; set; }
    public string Summery { get; set; }
    public string ObligationOrRequirement { get; set; }
    public long StatuteId { get; set; }
    public bool IsMandatory { get; set; }
    public bool ExcludeFromCompliance { get; set; }
    public decimal Coverage { get; set; }
    public bool IsCovered { get; set; }
    public decimal ComplianceAssurance { get; set; }
    public string Comments { get; set; }
    public long FrequencyId { get; set; }
    public virtual Frequency Frequency { get; set; }
    public virtual StatutoryRegulation Statute { get; set; }
    public virtual ICollection<RegulatoryReturn> Returns { get; set; }
    public virtual ICollection<ArticleRevision> ArticleRevisions { get; set; }
}
