using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class MiniProcessResponse {
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
