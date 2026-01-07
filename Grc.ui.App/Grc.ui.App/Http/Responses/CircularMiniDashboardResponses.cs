using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class CircularMiniDashboardResponses {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }

        [JsonPropertyName("circulars")]
        public List<GrcCircularReportResponse> Circulars { get; set; }

    }
}
