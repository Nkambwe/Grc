using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularDashboardResponse {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }

        [JsonPropertyName("circulars")]
        public List<CircularReportResponse> Circulars { get; set; }
    }
}
