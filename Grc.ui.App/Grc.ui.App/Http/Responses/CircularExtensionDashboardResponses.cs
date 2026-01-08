using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class CircularExtensionDashboardResponses {

        [JsonPropertyName("statuses")]
        public Dictionary<string, int> Statuses { get; set; }

        [JsonPropertyName("reports")]
        public List<GrcCircularReportResponse> Reports { get; set; }

    }
}
