using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class ComplianceMapRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapName")]
        public string MapDescription { get; set; }

        [JsonPropertyName("controlMaps")]
        public List<ControlMapRequest> ControlMaps { get; set; } = new();
    }
}
