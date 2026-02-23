using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcProcessVersionResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
