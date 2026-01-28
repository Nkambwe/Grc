using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularMetric {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }
}
