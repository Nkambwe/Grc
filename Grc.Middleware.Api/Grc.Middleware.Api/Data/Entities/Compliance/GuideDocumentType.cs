namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class GuideDocumentType : BaseEntity
    {
        public string DocumentType { get; set; }
        public virtual ICollection<GuideDocument> Documents { get; set; }
    }

}
