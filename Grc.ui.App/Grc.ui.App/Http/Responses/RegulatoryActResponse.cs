using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class RegulatoryActResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("regulatoryName")]
        public string RegulatoryName { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("regulatoryAuthority")]
        public string RegulatoryAuthority { get; set; }

        [JsonPropertyName("reviewFrequency")]
        public string ReviewFrequency { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("lastReviewDate")]
        public DateTime LastReviewDate { get; set; }

        [JsonPropertyName("reviewResponsibility")]
        public string ReviewResponsibility { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }
}
