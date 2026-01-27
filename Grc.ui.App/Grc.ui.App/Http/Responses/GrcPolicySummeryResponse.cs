using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcPolicySummeryResponse {
        [JsonPropertyName("count")]
        public Dictionary<string, int> Count { get; set; } = new();
        [JsonPropertyName("percentage")]
        public Dictionary<string, decimal> Percentage { get; set; } = new();
    }
}
