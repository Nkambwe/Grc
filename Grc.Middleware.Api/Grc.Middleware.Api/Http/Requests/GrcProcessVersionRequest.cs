using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class GrcProcessVersionRequest {
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
