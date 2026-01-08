using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class CircularDashboardResponses {

        [JsonPropertyName("authorities")]
        public Dictionary<string, int> Authorities { get; set; } = new();

        [JsonPropertyName("statuses")]
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();

    }
}
