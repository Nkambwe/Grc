using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcProcessActivityResponse {
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
