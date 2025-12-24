using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcComplianceMapRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapName")]
        public string MapDescription { get; set; }

        [JsonPropertyName("controlMaps")]
        public List<GrcControlMapRequest> ControlMaps { get; set; } = new();
    }
}