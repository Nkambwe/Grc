using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcReturnDashboardResponse {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }

        [JsonPropertyName("reports")]
        public List<GrcReturnReportResponse> Reports { get; set; }
    }
}
