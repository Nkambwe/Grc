using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcMiniProcessResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("currentVersion")]
        public string CurrentVersion { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }

    }
}
