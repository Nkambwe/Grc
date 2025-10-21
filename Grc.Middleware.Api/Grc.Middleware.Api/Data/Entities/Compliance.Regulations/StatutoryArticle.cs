using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;

public class StatutoryArticle : BaseEntity {
    public string Article { get; set; }
    public string Summery { get; set; }
    public string ObligationOrRequirement { get; set; }
    public long StatuteId { get; set; }
    public virtual StatutoryRegulation Statute { get; set; }
    public virtual ICollection<RegulatoryReturn> Returns { get; set; }
}
