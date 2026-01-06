using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ReturnsMiniDashboardResponses {

        [JsonPropertyName("returns")]
        public Dictionary<string, int> Returns { get; set; } = new();

    }

}
