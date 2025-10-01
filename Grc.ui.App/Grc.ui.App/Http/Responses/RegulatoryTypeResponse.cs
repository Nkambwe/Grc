namespace Grc.ui.App.Http.Responses
{
    public class RegulatoryTypeResponse
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
