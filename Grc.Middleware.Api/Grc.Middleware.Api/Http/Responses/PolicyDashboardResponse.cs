using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class PolicyDashboardResponse {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }

        [JsonPropertyName("policies")]
        public List<PolicyItemResponse> Policies { get; set; }
    }
}
