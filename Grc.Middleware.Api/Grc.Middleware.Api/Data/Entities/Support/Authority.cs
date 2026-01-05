using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class Authority : BaseEntity {
        public string AuthorityName { get; set; }
        public string AuthorityAlias { get; set; }
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
        public virtual ICollection<Audit> Audits { get; set; }
        public virtual ICollection<Circular> Circulars { get; set; }
        public virtual ICollection<ReturnReport> Returns { get; set; }
    }
}
