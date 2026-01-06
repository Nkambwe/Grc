using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ComplianceTaskStatisticResponse {

        [JsonPropertyName("totals")]
        public Dictionary<string, int> Totals { get; set; } = new();

        [JsonPropertyName("open")]
        public Dictionary<string, int> Open { get; set; } = new();

        [JsonPropertyName("closed")]
        public Dictionary<string, int> Closed { get; set; } = new();

        [JsonPropertyName("breached")]
        public Dictionary<string, int> Breached { get; set; } = new();


    }
}
