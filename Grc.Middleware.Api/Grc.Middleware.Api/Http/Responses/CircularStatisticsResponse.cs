using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularStatisticsResponse {
        [JsonPropertyName("authorities")]
        public Dictionary<string, int> Authorities { get; set; } = new();

        [JsonPropertyName("statuses")]
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();
    }
}
