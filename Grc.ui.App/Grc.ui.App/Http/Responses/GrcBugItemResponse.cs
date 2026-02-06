using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcBugItemResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("error ")]
        public string Error  { get; set; }
        
        [JsonPropertyName("severity ")]
        public string Severity  { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime? CreatedOn { get; set; }
    }
}
