using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class GrcComplianceMapViewMode {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapName")]
        public string MapDescription { get; set; }

        [JsonPropertyName("controlMaps")]
        public List<GrcControlMapViewModel> ControlMaps { get; set; } = new();
    }
}
