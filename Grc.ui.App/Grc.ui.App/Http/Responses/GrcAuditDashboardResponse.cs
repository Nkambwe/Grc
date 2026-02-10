using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcAuditDashboardResponse {

        [JsonPropertyName("findings")]
        public Dictionary<string, int> Findings { get; set; }

        [JsonPropertyName("completions")]
        public Dictionary<string, int> Completions { get; set; }

        [JsonPropertyName("barChart")]
        public Dictionary<string, Dictionary<string, int>> BarChart { get; set; }
    }
}
