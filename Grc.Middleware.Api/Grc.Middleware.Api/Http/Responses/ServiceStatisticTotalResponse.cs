using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceStatisticTotalResponse {

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("categories")]
        public Dictionary<string, int> Categories { get; set; } = new();
    }
}
