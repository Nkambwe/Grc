using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class BugItemResponse {

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
