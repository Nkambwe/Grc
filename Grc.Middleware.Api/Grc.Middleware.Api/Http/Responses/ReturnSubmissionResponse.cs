using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ReturnSubmissionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("risk")]
        public string Risk { get; set; }
    }
}
