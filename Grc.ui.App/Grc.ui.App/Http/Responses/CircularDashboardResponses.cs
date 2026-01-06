using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class CircularDashboardResponses {

        [JsonPropertyName("statuses")]
        public Dictionary<string, int> Statuses { get; set; } = new();

        [JsonPropertyName("authorities")]
        public Dictionary<string, int> Authorities { get; set; } = new();

    }
}
