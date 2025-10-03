namespace Grc.ui.App.Http.Responses
{
    public class RegulatoryCategoryResponse {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
