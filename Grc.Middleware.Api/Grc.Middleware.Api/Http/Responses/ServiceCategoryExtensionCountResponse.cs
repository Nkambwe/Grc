using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceCategoryExtensionCountResponse {

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("categoryProcesses")]
        public Dictionary<string, int> CategoryProcesses { get; set; } = new();
    }
}
