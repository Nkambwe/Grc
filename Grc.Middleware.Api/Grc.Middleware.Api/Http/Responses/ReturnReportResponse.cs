using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ReturnReportResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }
    }
}
