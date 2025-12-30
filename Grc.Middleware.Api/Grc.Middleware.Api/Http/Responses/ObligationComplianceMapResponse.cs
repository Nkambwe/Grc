using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ObligationComplianceMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("mapControl")]
        public string CategoryName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("items")]
        public List<ObligationComplianceItemResponse> Items { get; set; } = new();
    }
}
