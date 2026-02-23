using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ProcessActivityResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }
        
        [JsonPropertyName("activity")]
        public string Activity { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
    }
}
