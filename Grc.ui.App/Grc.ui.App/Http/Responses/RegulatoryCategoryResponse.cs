namespace Grc.ui.App.Http.Responses
{
    public class RegulatoryCategoryResponse {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PolicyRegisterResponse
    {
        public long Id { get; set; }
        public string PolicyName { get; set; }
        public string PolicyCode { get; set; }
        public string Owner { get; set; }
        public string DocumentType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime LastRevisionDate { get; set; }
        public DateTime NextRevisionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
