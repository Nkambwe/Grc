namespace Grc.ui.App.Http.Responses
{
    public class RegulatoryAuthorityResponse
    {
        public long Id { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityAlias { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
