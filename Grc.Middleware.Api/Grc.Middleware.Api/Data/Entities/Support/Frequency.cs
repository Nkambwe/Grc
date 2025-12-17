using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class Frequency : BaseEntity {
        public string FrequencyName { get; set; }
        public virtual ICollection<RegulatoryReturn> Returns { get; set; }
        public virtual ICollection<RegulatoryDocument> Documents { get; set; }
    }

}
