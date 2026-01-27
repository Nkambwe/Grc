using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class PolicySummeryResponse {
        [JsonPropertyName("count")]
        public Dictionary<string, int> Count { get; set; } = new();
        [JsonPropertyName("percentage")]
        public Dictionary<string, decimal> Percentage { get; set; } = new();
    }

}
