using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceUnitExtensionCountResponse {

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("unitProcesses")]
        public Dictionary<string, int> UnitProcesses { get; set; } = new();
    }
}
