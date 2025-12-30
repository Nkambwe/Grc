using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ComplianceMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapControl")]
        public string MapControl { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("controlMaps")]
        public List<ControlMapResponse> ControlMaps { get; set; } = new();
    }
}
