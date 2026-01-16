using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class AuditDashboardResponse {

        [JsonPropertyName("findings")]
        public Dictionary<string, int> Findings { get; set; }

        [JsonPropertyName("completions")]
        public Dictionary<string, int> Completions { get; set; }

        [JsonPropertyName("barChart")]
        public Dictionary<string, Dictionary<string, int>> BarChart { get; set; }
    }

}
