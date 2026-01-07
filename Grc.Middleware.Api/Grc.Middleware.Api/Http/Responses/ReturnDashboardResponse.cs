using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ReturnDashboardResponse {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }
        [JsonPropertyName("reports")]
        public List<ReturnReportResponse> Reports { get; set; }
    }
}
