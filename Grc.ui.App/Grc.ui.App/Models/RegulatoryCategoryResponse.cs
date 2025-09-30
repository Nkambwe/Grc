namespace Grc.ui.App.Models
{
    public class RegulatoryCategoryResponse {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
