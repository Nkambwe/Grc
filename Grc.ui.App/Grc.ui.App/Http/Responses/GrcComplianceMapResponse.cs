using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcComplianceMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapControl")]
        public string MapControl { get; set; }


        [JsonPropertyName("controlMaps")]
        public List<GrcControlMapResponse> ControlMaps { get; set; } = new();
    }
}
