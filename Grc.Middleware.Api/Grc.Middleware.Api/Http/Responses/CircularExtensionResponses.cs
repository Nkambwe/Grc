using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularExtensionResponses {

        [JsonPropertyName("statuses")]
        public Dictionary<string, int> Statuses { get; set; }

        [JsonPropertyName("reports")]
        public List<CircularExtensionResponse> Reports { get; set; }

    }
}
