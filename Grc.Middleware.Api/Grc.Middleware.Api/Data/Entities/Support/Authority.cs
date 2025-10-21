using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class Authority : BaseEntity {
        public string AuthorityName { get; set; }
        public string AuthorityAlias { get; set; }
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
        public virtual ICollection<Audit> Audits { get; set; }
    }
}
