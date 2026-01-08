using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ReturnsStatisticsResponses {

        [JsonPropertyName("periods")]
        public Dictionary<string, int> Periods { get; set; } = new();

        [JsonPropertyName("statuses")]
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();

    }
}
