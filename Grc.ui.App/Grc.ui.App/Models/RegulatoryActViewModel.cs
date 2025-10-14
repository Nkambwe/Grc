using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class RegulatoryActViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("regulatoryName")]
        public string RegulatoryAuthority { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("regulatoryAuthority")]
        public string RegulatoryName { get; set; }

        [JsonPropertyName("reviewFrequency")]
        public string ReviewFrequency { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("lastReviewDate")]
        public DateTime LastReviewDate { get; set; }

        [JsonPropertyName("reviewResponsibility")]
        public string AeviewResponsibility { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
    }
}
