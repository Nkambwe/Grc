using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceCategoriesCountResponse {

        [JsonPropertyName("unclassified")]
        public int Unclassified { get; set; }

        [JsonPropertyName("upToDate")]
        public int UpToDate { get; set; }

        [JsonPropertyName("unchanged")]
        public int Unchanged { get; set; }

        [JsonPropertyName("proposed")]
        public int Proposed { get; set; }

        [JsonPropertyName("due")]
        public int Due { get; set; }

        [JsonPropertyName("review")]
        public int InReview { get; set; }

        [JsonPropertyName("dormant")]
        public int Dormant { get; set; }

        [JsonPropertyName("cancelled")]
        public int Cancelled { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
