using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;

namespace Grc.Middleware.Api.Data.Entities.Support {
    public class RegulatoryCategory : BaseEntity {
        public string CategoryName { get; set; }
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
    }

}
