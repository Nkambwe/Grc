using Grc.ui.App.Defaults;
using Grc.ui.App.Models;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class TaskDashboardResponses {

        [JsonPropertyName("totals")]
        public Dictionary<string, int> Total { get; set; } = new();

        [JsonPropertyName("open")]
        public Dictionary<string, int> Open { get; set; }

        [JsonPropertyName("closed")]
        public Dictionary<string, int> Closed { get; set; }

        [JsonPropertyName("breached")]
        public Dictionary<string, int> Breached { get; set; }

    }
}
