using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;

namespace Grc.Middleware.Api.Data.Entities.Support {
    public class RegulatoryDocumentType : BaseEntity
    {
        public string DocumentType { get; set; }
        public virtual ICollection<RegulatoryDocument> Documents { get; set; }
    }

}
