using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GeneralComplianceReturnResponse {

        [JsonPropertyName("circulars")]
        public Dictionary<string, int> CircularStatuses { get; set; } = new();

        [JsonPropertyName("returns")]
        public Dictionary<string, int> ReturnStatuses { get; set; } = new();

        [JsonPropertyName("tasks")]
        public Dictionary<string, int> TaskStatuses { get; set; } = new();

        [JsonPropertyName("policies")]
        public Dictionary<string, int> Policies { get; set; } = new();
    }
}
