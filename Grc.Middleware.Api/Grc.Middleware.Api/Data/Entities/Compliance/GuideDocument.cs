namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class GuideDocument : BaseEntity {
        public string DocumentName { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        /// <summary>
        /// Check if document aligns with policy
        /// </summary>
        public bool PolicyAligned { get; set; }
        public DateTime LastRevisionDate { get; set; }
        public DateTime? NextRevisionDate { get; set; }
        public long DocumentTypeId { get; set; }
        public long ResponsibilityId { get; set; }
        public string Comments { get; set; }
        public virtual Responsibility Owner { get; set; }
        public virtual GuideDocumentType DocumentType { get; set; }
    }

}
