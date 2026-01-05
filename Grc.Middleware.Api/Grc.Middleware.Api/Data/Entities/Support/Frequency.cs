using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class Frequency : BaseEntity {
        public string FrequencyName { get; set; }
        public virtual ICollection<Circular> Circulars { get; set; }
        public virtual ICollection<StatutoryArticle> Articles { get; set; }
        public virtual ICollection<ReturnReport> Returns { get; set; }
        public virtual ICollection<RegulatoryDocument> Documents { get; set; }
    }

}
