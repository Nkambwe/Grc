using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ReturnExtensionResponse {

        [JsonPropertyName("periods")]
        public Dictionary<string, int> Periods { get; set; }

        [JsonPropertyName("reports")]
        public List<ReturnSubmissionResponse> Reports { get; set; }
    }
}
