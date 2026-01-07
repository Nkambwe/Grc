using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcPolicyDashboardResponse {

        [JsonPropertyName("statistics")]
        public Dictionary<string, int> Statistics { get; set; }

        [JsonPropertyName("policies")]
        public List<GrcPolicyItemResponse> Policies { get; set; }
    }
}
