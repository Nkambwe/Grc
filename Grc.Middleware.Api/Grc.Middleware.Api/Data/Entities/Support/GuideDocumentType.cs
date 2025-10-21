using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;

namespace Grc.Middleware.Api.Data.Entities.Support {
    public class GuideDocumentType : BaseEntity
    {
        public string DocumentType { get; set; }
        public virtual ICollection<GuideDocument> Documents { get; set; }
    }

}
